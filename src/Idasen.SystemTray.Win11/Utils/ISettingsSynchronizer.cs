using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Utils ;

public interface ISettingsSynchronizer
{
    bool HasParentalLockChanged ( ISettingsViewModel      model ) ;
    bool HaveAdvancedSettingsChanged ( ISettingsViewModel model ) ;
    void UpdateCurrentSettings ( ISettingsViewModel       model ) ;
    Task LoadSettingsAsync ( ISettingsViewModel           model , CancellationToken token ) ;
    Task StoreSettingsAsync ( ISettingsViewModel          model , CancellationToken token ) ;
    void ChangeTheme ( string                             parameter ) ;
}