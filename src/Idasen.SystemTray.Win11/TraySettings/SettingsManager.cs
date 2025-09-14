using System.IO ;
using System.IO.Abstractions ;
using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsManager ( ILogger                logger ,
                               ICommonApplicationData commonApplicationData ,
                               ISettingsStorage       settingsStorage ,
                               IFileSystem            fileSystem )
    : ISettingsManager
{
    private readonly ISubject < ISettings > _settingsSaved = new Subject < ISettings > ( ) ;

    private Settings _current = new ( ) ;

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
            {
                await AddMissingSettingsNotificationsEnabled ( token ).ConfigureAwait ( false ) ;
            }

            return true ; // Upgrade not needed or successfully completed
        }
        catch ( Exception e )
        {
            logger.Error ( e , "Failed to upgrade settings" ) ;
            return false ;
        }
    }

    private static bool MissingNotificationsEnabled ( string settingsJson )
        => ! settingsJson.Contains ( nameof ( Settings.DeviceSettings.NotificationsEnabled ) , StringComparison.Ordinal ) ;

    public async Task SetLastKnownDeskHeight ( uint heightInCm , CancellationToken token )
    {
        CurrentSettings.HeightSettings.LastKnownDeskHeight = heightInCm ;

        await SaveAsync ( token ).ConfigureAwait ( false ) ;
    }

    public async Task LoadAsync ( CancellationToken token )
    {
        _current = await settingsStorage.LoadSettingsAsync ( SettingsFileName, token ).ConfigureAwait ( false ) ;
    }

    private async Task AddMissingSettingsNotificationsEnabled ( CancellationToken token )
    {
        logger.Debug ( $"Add missing setting "                                       +
                       $"{nameof ( Settings.DeviceSettings.NotificationsEnabled )} " +
                       $"to current settings from '{SettingsFileName}'" ) ;

        await LoadAsync ( token ).ConfigureAwait ( false ) ;

        _current.DeviceSettings.NotificationsEnabled = Constants.NotificationsEnabled ;

        await SaveAsync ( token ).ConfigureAwait ( false ) ;
    }
}