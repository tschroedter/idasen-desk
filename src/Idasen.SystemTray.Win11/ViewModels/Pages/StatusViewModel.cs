using System.Reactive.Concurrency ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class StatusViewModel : StatusBarInfoViewModelBase
{
    public StatusViewModel ( IUiDeskManager                                                   manager ,
                             IScheduler                                                       scheduler ,
                             Func < TimerCallback , object ? , TimeSpan , TimeSpan , ITimer > timerFactory )
        : base ( manager ,
                 scheduler ,
                 timerFactory )
    {
        InitializeFromLastStatusBarInfo ( ) ;
    }
}