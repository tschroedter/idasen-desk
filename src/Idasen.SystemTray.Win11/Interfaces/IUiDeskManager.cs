using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IUiDeskManager : IDisposable
{
    bool                          IsInitialize         { get ; }
    IObservable < StatusBarInfo > StatusBarInfoChanged { get ; }
    StatusBarInfo                 LastStatusBarInfo    { get ; }
    bool                          IsConnected          { get ; }
    UiDeskManager                 Initialize ( NotifyIcon notifyIcon ) ;
    Task                          StandAsync ( ) ;
    Task                          SitAsync ( ) ;
    Task                          AutoConnectAsync ( ) ;
    Task                          DisconnectAsync ( ) ;
    Task                          HideAsync ( ) ;
    Task                          ExitAsync ( ) ;
    Task                          StopAsync ( ) ;
    Task                          MoveLockAsync ( ) ;
    Task                          MoveUnlockAsync ( ) ;
}