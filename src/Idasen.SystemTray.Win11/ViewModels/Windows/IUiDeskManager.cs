using Autofac ;
using Hardcodet.Wpf.TaskbarNotification ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public interface IUiDeskManager : IDisposable
{
    bool                   IsInitialize  { get ; }
    IObservable < uint >   HeightChanged { get ; }
    IObservable < uint >   Finished      { get ; }
    IObservable < string > Error         { get ; }
    IObservable < string > Connected     { get ; }
    UiDeskManager        Initialize ( IContainer container , TaskbarIcon taskbarIcon ) ;
    Task                 Stand ( ) ;
    Task                 Sit ( ) ;
    Task                 AutoConnect ( ) ;
    Task                 Disconnect ( ) ;
}