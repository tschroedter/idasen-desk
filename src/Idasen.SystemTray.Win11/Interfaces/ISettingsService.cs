using Idasen.SystemTray.Win11.Utils ;

namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Composite service for settings management operations.
/// </summary>
public interface ISettingsService
{
    ILoggingSettingsManager  SettingsManager { get ; }
    ISettingsSynchronizer    Synchronizer    { get ; }
    IHeightSettingsValidator HeightValidator { get ; }
}
