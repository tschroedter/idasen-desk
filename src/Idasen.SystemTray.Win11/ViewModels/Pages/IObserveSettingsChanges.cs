namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public interface IObserveSettingsChanges
{
    IObservable<bool>    AdvancedSettingsChanged { get ; }
    IObservable < bool > LockSettingsChanged     { get ; }
}