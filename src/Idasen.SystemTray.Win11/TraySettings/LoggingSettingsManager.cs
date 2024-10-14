using Autofac ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.TraySettings;

/* todo
 *    public interface ISettingsWindow
   {
       void                                             Show ( ) ;
       void                                             Close ( ) ;

       event EventHandler                               AdvancedSettingsChanged ;
       event EventHandler<LockSettingsChangedEventArgs> LockSettingsChanged ;
   }
 */
public class LoggingSettingsManager ( ISettingsManager settingsManager )
    : ILoggingSettingsManager // todo: remove ILoggingSettingsManager
{
    private ILogger ? _logger ;
    public  ISettings CurrentSettings  => settingsManager.CurrentSettings ;
    public  string    SettingsFileName => settingsManager.SettingsFileName ;

    public async Task Save ( )
    {
        try
        {
            _logger?.Debug ( $"Saving current setting [{CurrentSettings}] to '{SettingsFileName}'" ) ;

            await settingsManager.Save ( ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e , $"Failed to save settings in file '{SettingsFileName}'" ) ;

            throw ;
        }
    }

    public async Task Load ( )
    {
        try
        {
            _logger?.Debug ( $"Loading setting from '{SettingsFileName}'" ) ;

            await settingsManager.Load ( ) ;

            _logger?.Debug ( $"Settings loaded: {CurrentSettings}" ) ;
        }
        catch ( Exception e )
        {
            _logger?.Error ( e , "Failed to load settings" ) ;
        }
    }

    public async Task < bool > UpgradeSettings ( )
    {
        try
        {
            _logger?.Debug ( $"Check current setting from '{SettingsFileName}'" ) ;

            var success = await settingsManager.UpgradeSettings ( ) ;

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
        _logger = container.Resolve < ILogger > ( ) ;
    }
}