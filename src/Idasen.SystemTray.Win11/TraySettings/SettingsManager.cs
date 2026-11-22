using System.IO.Abstractions ;
using System.Reactive.Subjects ;
using System.Text.Json ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public sealed class SettingsManager (
    ILogger                logger ,
    ICommonApplicationData commonApplicationData ,
    ISettingsStorage       settingsStorage ,
    IFileSystem            fileSystem )
    : ISettingsManager , IDisposable
{
    private readonly Subject < ISettings > _settingsSaved = new( ) ;

    private Settings _current = new( ) ;

    public void Dispose ( )
    {
        _settingsSaved.Dispose ( ) ;
    }

    public IObservable < ISettings > SettingsSaved => _settingsSaved ;

    public ISettings CurrentSettings => _current ;

    public string SettingsFileName { get ; } =
        commonApplicationData.ToFullPath ( AppConfiguration.Application.SettingsFileName ) ;

    public async ValueTask SaveAsync ( CancellationToken token )
    {
        await settingsStorage.SaveSettingsAsync ( SettingsFileName ,
                                                  _current ,
                                                  token ).ConfigureAwait ( false ) ;

        _settingsSaved.OnNext ( _current ) ;
    }

    public async ValueTask < bool > UpgradeSettingsAsync ( CancellationToken token )
    {
        try
        {
            token.ThrowIfCancellationRequested ( ) ;

            if ( ! fileSystem.File.Exists ( SettingsFileName ) )
                return true ; // Nothing to upgrade

            var settingsJson = await fileSystem.File.ReadAllTextAsync ( SettingsFileName ,
                                                                        token ).ConfigureAwait ( false ) ;

            if ( MissingNotificationsEnabled ( settingsJson ) )
                await AddMissingSettingsNotificationsEnabled ( token ).ConfigureAwait ( false ) ;

            return true ; // Upgrade not needed or successfully completed
        }
        catch ( OperationCanceledException ) when ( token.IsCancellationRequested )
        {
            throw ;
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to upgrade settings" ) ;
            return false ;
        }
    }

    public async ValueTask SetLastKnownDeskHeight ( uint heightInCm , CancellationToken token )
    {
        token.ThrowIfCancellationRequested ( ) ;
        CurrentSettings.HeightSettings.LastKnownDeskHeight = heightInCm ;

        await SaveAsync ( token ).ConfigureAwait ( false ) ;
    }

    public async ValueTask LoadAsync ( CancellationToken token )
    {
        _current = await settingsStorage.LoadSettingsAsync ( SettingsFileName ,
                                                             token ).ConfigureAwait ( false ) ;
    }

    public async ValueTask ResetSettingsAsync ( CancellationToken token )
    {
        try
        {
            token.ThrowIfCancellationRequested ( ) ;
            logger.Information ( "Resetting settings to defaults" ) ;

            // Create a fresh default settings instance and align theme with current Windows theme
            _current = new Settings
            {
                AppearanceSettings = { ThemeName = ThemeDefaults.GetDefaultThemeName ( ) }
            } ;

            await SaveAsync ( token ).ConfigureAwait ( false ) ;
        }
        catch ( OperationCanceledException ) when ( token.IsCancellationRequested )
        {
            throw ;
        }
        catch ( Exception ex )
        {
            logger.Error ( ex ,
                           "Failed to reset settings" ) ;

            throw new InvalidOperationException ( "Failed to reset settings" ,
                                                  ex ) ;
        }
    }

    private static bool MissingNotificationsEnabled ( string settingsJson )
    {
        using var document = JsonDocument.Parse ( settingsJson ) ;

        return ! TryGetProperty ( document.RootElement ,
                                 nameof ( Settings.DeviceSettings ) ,
                                 out var deviceSettings ) ||
               ! TryGetProperty ( deviceSettings ,
                                  nameof ( DeviceSettings.NotificationsEnabled ) ,
                                  out _ ) ;
    }

    private static bool TryGetProperty ( JsonElement element ,
                                         string propertyName ,
                                         out JsonElement propertyValue )
    {
        if ( element.ValueKind == JsonValueKind.Object )
            foreach ( var property in element.EnumerateObject ( ) )
                if ( string.Equals ( property.Name ,
                                     propertyName ,
                                     StringComparison.OrdinalIgnoreCase ) )
                {
                    propertyValue = property.Value ;
                    return true ;
                }

        propertyValue = default ;
        return false ;
    }

    private async ValueTask AddMissingSettingsNotificationsEnabled ( CancellationToken token )
    {
        logger.Debug ( "Adding missing setting NotificationsEnabled to settings file {SettingsFileName}" ,
                       SettingsFileName ) ;

        await LoadAsync ( token ).ConfigureAwait ( false ) ;

        _current.DeviceSettings.NotificationsEnabled = AppConfiguration.Defaults.NotificationsEnabled ;

        await SaveAsync ( token ).ConfigureAwait ( false ) ;
    }
}
