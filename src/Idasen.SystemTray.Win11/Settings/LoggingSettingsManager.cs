using Serilog ;

namespace Idasen.SystemTray.Win11.Settings ;

public class LoggingSettingsManager : ILoggingSettingsManager
{
    private readonly ISettingsManager _settingsManager ;
    private readonly ILogger          _logger ;

    public LoggingSettingsManager ( ILogger          logger,
                                    ISettingsManager settingsManager )
    {
        _settingsManager  = settingsManager ;
        _logger           = logger ;
    }

    public ISettings CurrentSettings  => _settingsManager.CurrentSettings ;
    public string    SettingsFileName => _settingsManager.SettingsFileName ;

    public async Task Save ( )
    {
        try
        {
            _logger.Debug ( $"Saving current setting [{CurrentSettings}] to '{SettingsFileName}'" ) ;

            await _settingsManager.Save ( ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e , $"Failed to save settings in file '{SettingsFileName}'" ) ;

            throw ;
        }
    }

    public async Task Load ( )
    {
        try
        {
            _logger.Debug ( $"Loading setting from '{SettingsFileName}'" ) ;

            await _settingsManager.Load ( ) ;

            _logger.Debug ( $"Settings loaded: {CurrentSettings}" ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e , "Failed to load settings" ) ;
        }
    }

    public async Task < bool > UpgradeSettings ( )
    {
        try
        {
            _logger.Debug ( $"Check current setting from '{SettingsFileName}'" ) ;

            var success = await _settingsManager.UpgradeSettings ( ) ;

            if ( success )
            {
                _logger.Debug ( $"Upgrade check completed for current setting from '{SettingsFileName}'" ) ;
            }
            else
            {
                _logger.Error ( $"Failed to upgrade current settings from '{SettingsFileName}'" ) ;
            }

            return success ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e , "Failed to upgrade settings" ) ;
            
            return false ;
        }
    }
}