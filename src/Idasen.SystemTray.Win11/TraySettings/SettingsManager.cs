using System.IO.Abstractions ;
using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public partial class SettingsManager (
    ILogger                logger ,
    ICommonApplicationData commonApplicationData ,
    ISettingsStorage       settingsStorage ,
    IFileSystem            fileSystem )
    : ISettingsManager , IDisposable
{
    private readonly Subject < ISettings > _settingsSaved = new( ) ;

    private Settings _current = new( ) ;
    private bool     _disposed ;

    public void Dispose ( )
    {
        Dispose ( true ) ;

        GC.SuppressFinalize ( this ) ;
    }

    public IObservable < ISettings > SettingsSaved => _settingsSaved ;

    public ISettings CurrentSettings => _current ;

    public string SettingsFileName { get ; } = commonApplicationData.ToFullPath ( Constants.SettingsFileName ) ;

    public async Task SaveAsync ( CancellationToken token )
    {
        await settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                  _current ,
                                                  token ).ConfigureAwait ( false ) ;

        _settingsSaved.OnNext ( _current ) ;
    }

    public async Task < bool > UpgradeSettingsAsync ( CancellationToken token )
    {
        try
        {
            if ( ! fileSystem.File.Exists ( SettingsFileName ) )
                return true ; // Nothing to upgrade

            var settingsJson = await fileSystem.File.ReadAllTextAsync ( SettingsFileName ,
                                                                        token ).ConfigureAwait ( false ) ;

            if ( MissingNotificationsEnabled ( settingsJson ) )
                await AddMissingSettingsNotificationsEnabled ( token ).ConfigureAwait ( false ) ;

            return true ; // Upgrade not needed or successfully completed
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to upgrade settings" ) ;
            return false ;
        }
    }

    public async Task SetLastKnownDeskHeight ( uint heightInCm , CancellationToken token )
    {
        CurrentSettings.HeightSettings.LastKnownDeskHeight = heightInCm ;

        await SaveAsync ( token ).ConfigureAwait ( false ) ;
    }

    public async Task LoadAsync ( CancellationToken token )
    {
        _current = await settingsStorage.LoadSettingsAsync ( SettingsFileName ,
                                                             token ).ConfigureAwait ( false ) ;
    }

    public async Task ResetSettingsAsync ( CancellationToken token )
    {
        try
        {
            logger.Information ( "Resetting settings to defaults" ) ;

            // Create a fresh default settings instance
            _current = new Settings ( ) ;

            await SaveAsync ( token ).ConfigureAwait ( false ) ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "Failed to reset settings" ) ;

            throw new InvalidOperationException ( "Failed to reset settings" ) ;
        }
    }

    private static bool MissingNotificationsEnabled ( string settingsJson )
    {
        return ! settingsJson.Contains ( nameof ( Settings.DeviceSettings.NotificationsEnabled ) ,
                                         StringComparison.Ordinal ) ;
    }

    private async Task AddMissingSettingsNotificationsEnabled ( CancellationToken token )
    {
        logger.Debug ( "Adding missing setting NotificationsEnabled to settings file {SettingsFileName}" ,
                       SettingsFileName ) ;

        await LoadAsync ( token ).ConfigureAwait ( false ) ;

        _current.DeviceSettings.NotificationsEnabled = Constants.NotificationsEnabled ;

        await SaveAsync ( token ).ConfigureAwait ( false ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing ) _settingsSaved.Dispose ( ) ;

        _disposed = true ;
    }
}