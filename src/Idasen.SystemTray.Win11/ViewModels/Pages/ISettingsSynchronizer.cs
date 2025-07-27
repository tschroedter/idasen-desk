namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public interface ISettingsSynchronizer
{
    bool HasParentalLockChanged ( SettingsViewModel      model ) ;
    bool HaveAdvancedSettingsChanged ( SettingsViewModel model ) ;
    void UpdateCurrentSettings ( SettingsViewModel       model ) ;
    Task LoadSettingsAsync ( SettingsViewModel           model , CancellationToken token ) ;
    Task StoreSettingsAsync ( SettingsViewModel          model , CancellationToken token ) ;
}