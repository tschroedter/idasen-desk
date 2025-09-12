namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IObserveSettingsChanges
{
    IObservable < bool >           AdvancedSettingsChanged { get ; }
    IObservable < bool >           LockSettingsChanged     { get ; }
}