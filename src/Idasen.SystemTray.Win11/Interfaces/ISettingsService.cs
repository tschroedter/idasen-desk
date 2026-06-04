namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Composite service for settings management operations.
/// </summary>
public interface ISettingsService
{
    ILoggingSettingsManager SettingsManager { get ; }
    Utils.ISettingsSynchronizer Synchronizer { get ; }
    IHeightSettingsValidator HeightValidator { get ; }
}
