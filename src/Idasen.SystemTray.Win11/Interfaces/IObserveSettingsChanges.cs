using Idasen.SystemTray.Win11.TraySettings ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IObserveSettingsChanges
{
    IObservable < bool >           AdvancedSettingsChanged { get ; }
    IObservable < bool >           LockSettingsChanged     { get ; }
    IObservable < HeightSettings > HeightSettingsChanged   { get ; }
}