using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class LoggingSettingsManager ( ILogger          logger ,
                                      ISettingsManager settingsManager )
    : ILoggingSettingsManager
{
    public ISettings                 CurrentSettings  => settingsManager.CurrentSettings ;
    public string                    SettingsFileName => settingsManager.SettingsFileName ;
    public IObservable < ISettings > SettingsSaved    => settingsManager.SettingsSaved ;

    public async Task SaveAsync ( CancellationToken token )
    {
        try
        {
            logger.Debug ( $"Saving current setting [{CurrentSettings}] to '{SettingsFileName}'" ) ;

            await settingsManager.SaveAsync ( token ) ;
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           $"Failed to save settings in file '{SettingsFileName}'" ) ;

            throw ;
        }
    }

    public async Task LoadAsync ( CancellationToken token )
    {
        try
        {
            logger.Debug ( $"Loading setting from '{SettingsFileName}'" ) ;

            await settingsManager.LoadAsync ( token ) ;

            logger.Debug ( $"Settings loaded: {CurrentSettings}" ) ;
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to load settings" ) ;
        }
    }

    public async Task < bool > UpgradeSettingsAsync ( CancellationToken token )
    {
        try
        {
            logger.Debug ( $"Check current setting from '{SettingsFileName}'" ) ;

            var success = await settingsManager.UpgradeSettingsAsync ( token ) ;

            if ( success )
            {
                logger.Debug ( $"Upgrade check completed for current setting from '{SettingsFileName}'" ) ;
            }
            else
            {
                logger.Error ( $"Failed to upgrade current settings from '{SettingsFileName}'" ) ;
            }

            return success ;
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
        logger.Debug ( $"{nameof ( SettingsManager )} updating last known desk height: {heightInCm}" ) ;

        await settingsManager.SetLastKnownDeskHeight ( heightInCm ,
                                                       token ) ;
    }
}