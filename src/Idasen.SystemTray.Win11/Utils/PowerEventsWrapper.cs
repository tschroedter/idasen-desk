using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Win32 ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class PowerEventsWrapper : IPowerEvents
{
    private bool _disposed ;

    public PowerEventsWrapper ( )
    {
        SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged ;
    }

    public event PowerModeChangedEventHandler ? PowerModeChanged ;

    public void Dispose ( )
    {
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing ) SystemEvents.PowerModeChanged -= SystemEvents_PowerModeChanged ;

        _disposed = true ;
    }

    private void SystemEvents_PowerModeChanged ( object ? sender , PowerModeChangedEventArgs e )
    {
        PowerModeChanged?.Invoke ( sender ,
                                   e ) ;
    }
}