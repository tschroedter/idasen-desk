namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Composite implementation of settings service that aggregates settings-related dependencies.
/// </summary>
public class SettingsService : Interfaces.ISettingsService
{
    public SettingsService (
        Interfaces.ILoggingSettingsManager settingsManager ,
        ISettingsSynchronizer              synchronizer ,
        Interfaces.IHeightSettingsValidator heightValidator )
    {
        SettingsManager  = settingsManager ;
        Synchronizer     = synchronizer ;
        HeightValidator  = heightValidator ;
    }

    public Interfaces.ILoggingSettingsManager SettingsManager { get ; }

    public ISettingsSynchronizer Synchronizer { get ; }

    public Interfaces.IHeightSettingsValidator HeightValidator { get ; }
}
