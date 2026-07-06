using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

/// <summary>
///     Composite implementation of settings service that aggregates settings-related dependencies.
/// </summary>
[ ExcludeFromCodeCoverage ]
public class SettingsService (
    ILoggingSettingsManager  settingsManager ,
    ISettingsSynchronizer    synchronizer ,
    IHeightSettingsValidator heightValidator )
    : ISettingsService
{
    public ILoggingSettingsManager SettingsManager { get ; } = settingsManager ;

    public ISettingsSynchronizer Synchronizer { get ; } = synchronizer ;

    public IHeightSettingsValidator HeightValidator { get ; } = heightValidator ;
}
