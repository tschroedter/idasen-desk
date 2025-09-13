using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsChanges : IObserveSettingsChanges , INotifySettingsChanges , IDisposable
{
    private readonly Subject < bool > _lockSettingsChanged     = new ( ) ;
    private readonly Subject < bool > _advancedSettingsChanged = new ( ) ;

    public IObservable < bool > AdvancedSettingsChanged => _advancedSettingsChanged ;
    public IObservable < bool > LockSettingsChanged     => _lockSettingsChanged ;

    public void NotifyAdvancedSettingsChanged ( bool hasChanged ) => _advancedSettingsChanged.OnNext ( hasChanged ) ;
    public void NotifyLockSettingsChanged     ( bool isLocked )   => _lockSettingsChanged.OnNext ( isLocked ) ;

    public void Dispose ( )
    {
        _lockSettingsChanged .OnCompleted ( ) ;
        _advancedSettingsChanged.OnCompleted ( ) ;

        _lockSettingsChanged .Dispose ( ) ;
        _advancedSettingsChanged.Dispose ( ) ;
    }
}