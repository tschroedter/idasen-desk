using Autofac ;
using Hardcodet.Wpf.TaskbarNotification ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public interface IUiDeskManager : IDisposable
{
    bool                          IsInitialize         { get ; }
    IObservable < StatusBarInfo > StatusBarInfoChanged { get ; }
    StatusBarInfo LastStatusBarInfo    { get ; }
    UiDeskManager                 Initialize ( IContainer container , TaskbarIcon taskbarIcon ) ;
    Task                          Stand ( ) ;
    Task                          Sit ( ) ;
    Task                          AutoConnect ( ) ;
    Task                          Disconnect ( ) ;
}