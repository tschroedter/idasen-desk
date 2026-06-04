namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Composite implementation of settings service that aggregates settings-related dependencies.
/// </summary>
public class SettingsService ( Interfaces.ILoggingSettingsManager  settingsManager ,
                               ISettingsSynchronizer               synchronizer ,
                               Interfaces.IHeightSettingsValidator heightValidator )
    : Interfaces.ISettingsService
{
    public Interfaces.ILoggingSettingsManager SettingsManager { get ; } = settingsManager ;

    public ISettingsSynchronizer Synchronizer { get ; } = synchronizer ;

    public Interfaces.IHeightSettingsValidator HeightValidator { get ; } = heightValidator ;
}
