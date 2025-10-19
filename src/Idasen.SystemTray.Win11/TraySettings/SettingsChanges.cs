using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public partial class SettingsChanges : IObserveSettingsChanges , INotifySettingsChanges , IDisposable
{
    private readonly Subject < bool > _advancedSubject = new( ) ;
    private readonly Subject < bool > _lockSubject     = new( ) ;

    private bool _disposed ;

    public void Dispose ( )
    {
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    public ISubject < bool > LockSettingsChanged     => _lockSubject ;
    public ISubject < bool > AdvancedSettingsChanged => _advancedSubject ;

    IObservable < bool > IObserveSettingsChanges.AdvancedSettingsChanged => _advancedSubject ;
    IObservable < bool > IObserveSettingsChanges.LockSettingsChanged     => _lockSubject ;

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing )
        {
            _lockSubject.OnCompleted ( ) ;
            _advancedSubject.OnCompleted ( ) ;

            _lockSubject.Dispose ( ) ;
            _advancedSubject.Dispose ( ) ;
        }

        _disposed = true ;
    }
}