using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public partial class LoggingSettingsManager (
    ILogger          logger ,
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
            logger.Debug ( "Saving current settings to file {SettingsFileName}. Settings: {Settings}" ,
                           SettingsFileName ,
                           CurrentSettings ) ;

            await settingsManager.SaveAsync ( token ).ConfigureAwait ( false ) ;
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to save settings in file {SettingsFileName}" ,
                           SettingsFileName ) ;

            throw new InvalidOperationException ( $"Failed to save settings in file {SettingsFileName}" ) ;
        }
    }

    public async Task LoadAsync ( CancellationToken token )
    {
        try
        {
            logger.Debug ( "Loading settings from {SettingsFileName}" ,
                           SettingsFileName ) ;

            await settingsManager.LoadAsync ( token ).ConfigureAwait ( false ) ;

            logger.Debug ( "Settings loaded: {Settings}" ,
                           CurrentSettings ) ;
        }
        catch ( Exception e )
        {
            logger.Error ( e ,
                           "Failed to load settings" ) ;

            throw new InvalidOperationException ( $"Failed to load settings from file {SettingsFileName}" ) ;
        }
    }

    public async Task < bool > UpgradeSettingsAsync ( CancellationToken token )
    {
        try
        {
            logger.Debug ( "Checking for settings upgrade in {SettingsFileName}" ,
                           SettingsFileName ) ;

            var success = await settingsManager.UpgradeSettingsAsync ( token ).ConfigureAwait ( false ) ;

            if ( success )
                logger.Debug ( "Upgrade check completed for {SettingsFileName}" ,
                               SettingsFileName ) ;
            else
                logger.Error ( "Failed to upgrade settings from {SettingsFileName}" ,
                               SettingsFileName ) ;

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
        logger.Debug ( "{Component} updating last known desk height: {HeightInCm}" ,
                       nameof ( SettingsManager ) ,
                       heightInCm ) ;

        await settingsManager.SetLastKnownDeskHeight ( heightInCm ,
                                                       token ).ConfigureAwait ( false ) ;
    }

    public Task ResetSettingsAsync ( CancellationToken token )
    {
        logger.Information ( "Resetting settings to default" ) ;

        return settingsManager.ResetSettingsAsync ( token ) ;
    }
}