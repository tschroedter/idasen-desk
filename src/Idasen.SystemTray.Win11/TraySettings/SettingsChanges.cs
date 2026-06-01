using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.TraySettings ;

public partial class SettingsChanges : IObserveSettingsChanges , INotifySettingsChanges , IDisposable
{
    private readonly Subject < bool > _advancedSubject = new( ) ;
    private readonly Subject < bool > _lockSubject     = new( ) ;
    private readonly Subject < bool > _hotkeySubject   = new( ) ;
    private readonly Subject < bool > _heightSubject   = new( ) ;

    private bool _disposed ;

    public void Dispose ( )
    {
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    public ISubject < bool > LockSettingsChanged     => _lockSubject ;
    public ISubject < bool > AdvancedSettingsChanged => _advancedSubject ;
    public ISubject < bool > HotkeySettingsChanged   => _hotkeySubject ;
    public ISubject < bool > HeightSettingsChanged   => _heightSubject ;

    IObservable < bool > IObserveSettingsChanges.AdvancedSettingsChanged => _advancedSubject ;
    IObservable < bool > IObserveSettingsChanges.LockSettingsChanged     => _lockSubject ;
    IObservable < bool > IObserveSettingsChanges.HotkeySettingsChanged   => _hotkeySubject ;
    IObservable < bool > IObserveSettingsChanges.HeightSettingsChanged   => _heightSubject ;

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing )
        {
            _lockSubject.OnCompleted ( ) ;
            _advancedSubject.OnCompleted ( ) ;
            _hotkeySubject.OnCompleted ( ) ;
            _heightSubject.OnCompleted ( ) ;

            _lockSubject.Dispose ( ) ;
            _advancedSubject.Dispose ( ) ;
            _hotkeySubject.Dispose ( ) ;
            _heightSubject.Dispose ( ) ;
        }

        _disposed = true ;
    }
}