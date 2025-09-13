using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public class SettingsChanges : IObserveSettingsChanges , INotifySettingsChanges , IDisposable
{
    private readonly Subject < bool > _lockSubject     = new ( ) ;
    private readonly Subject < bool > _advancedSubject = new ( ) ;

    public ISubject < bool > LockSettingsChanged     => _lockSubject ;
    public ISubject < bool > AdvancedSettingsChanged => _advancedSubject ;

    IObservable < bool > IObserveSettingsChanges.AdvancedSettingsChanged => _advancedSubject ;
    IObservable < bool > IObserveSettingsChanges.LockSettingsChanged     => _lockSubject ;

    public void Dispose ( )
    {
        _lockSubject.OnCompleted ( ) ;
        _advancedSubject.OnCompleted ( ) ;

        _lockSubject.Dispose ( ) ;
        _advancedSubject.Dispose ( ) ;
    }
}