using Microsoft.Win32 ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IPowerEvents : IDisposable
{
    event PowerModeChangedEventHandler ? PowerModeChanged ;
}