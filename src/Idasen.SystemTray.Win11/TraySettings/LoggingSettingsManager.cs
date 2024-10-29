using Autofac ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class LoggingSettingsManager ( ISettingsManager settingsManager )
    : ILoggingSettingsManager
{
    private ILogger ?                 _logger ;
    public  ISettings                 CurrentSettings  => settingsManager.CurrentSettings ;
    public  string                    SettingsFileName => settingsManager.SettingsFileName ;
    public  IObservable < ISettings > SettingsSaved    => settingsManager.SettingsSaved ;

    public async Task SaveAsync ( )
    {
        try
        {
            _logger?.Debug ( $"Saving current setting [{CurrentSettings}] to '{SettingsFileName}'" ) ;

            await settingsManager.SaveAsync ( ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e , $"Failed to save settings in file '{SettingsFileName}'" ) ;

            throw ;
        }
    }

    public async Task LoadAsync ( )
    {
        try
        {
            _logger?.Debug ( $"Loading setting from '{SettingsFileName}'" ) ;

            await settingsManager.LoadAsync ( ) ;

            _logger?.Debug ( $"Settings loaded: {CurrentSettings}" ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e , "Failed to load settings" ) ;
        }
    }

    public async Task < bool > UpgradeSettingsAsync ( )
    {
        try
        {
            _logger?.Debug ( $"Check current setting from '{SettingsFileName}'" ) ;

            var success = await settingsManager.UpgradeSettingsAsync ( ) ;

            if ( success )
            {
                _logger?.Debug ( $"Upgrade check completed for current setting from '{SettingsFileName}'" ) ;
            }
            else
            {
                _logger?.Error ( $"Failed to upgrade current settings from '{SettingsFileName}'" ) ;
            }

            return success ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e , "Failed to upgrade settings" ) ;

            return false ;
        }
    }

    public void Initialize ( IContainer container )
    {
        settingsManager.Initialize( container ) ;

        _logger = container.Resolve<ILogger>();

        _logger?.Debug($"{nameof(SettingsManager)} initializing...");
    }

    public async Task SetLastKnownDeskHeight ( uint heightInCm )
    {
        _logger?.Debug($"{nameof(SettingsManager)} updating last known desk height: {heightInCm}");

        await settingsManager.SetLastKnownDeskHeight ( heightInCm ) ;
    }
}