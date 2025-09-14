using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

public interface ISettingsSynchronizer
{
    Task LoadSettingsAsync ( ISettingsViewModel  model , CancellationToken token ) ;
    Task StoreSettingsAsync ( ISettingsViewModel model , CancellationToken token ) ;
    void ChangeTheme ( string                    parameter ) ;
}