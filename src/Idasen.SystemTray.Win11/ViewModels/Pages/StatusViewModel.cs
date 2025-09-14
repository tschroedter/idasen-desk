using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class StatusViewModel : ObservableObject , IDisposable
{
    [ ObservableProperty ]
    private uint _height ;

    private IUiDeskManager ? _manager ;

    [ ObservableProperty ]
    private string _message = "Unknown" ;

    [ ObservableProperty ]
    private InfoBarSeverity _severity = InfoBarSeverity.Informational ;

    private IDisposable ? _statusBarInfoChanged ;
    private ITimer ?      _timer ;

    [ ObservableProperty ]
    private string _title = "Desk Status" ;

    public StatusViewModel ( IUiDeskManager                                                   manager ,
                             IScheduler                                                       scheduler ,
                             Func < TimerCallback , object ? , TimeSpan , TimeSpan , ITimer > timerFactory )
    {
        var timer = timerFactory ( OnElapsed ,
                                   null ,
                                   TimeSpan.FromSeconds ( 10 ) ,
                                   Timeout.InfiniteTimeSpan ) ;

        Init ( manager ,
               scheduler ,
               timer ) ;
    }

    public void Dispose ( )
    {
        _timer?.Dispose ( ) ;
        _statusBarInfoChanged?.Dispose ( ) ;
    }

    private void Init ( IUiDeskManager manager ,
                        IScheduler     scheduler ,
                        ITimer         timer )
    {
        _manager = manager ;

        _statusBarInfoChanged = manager.StatusBarInfoChanged
                                       .ObserveOn ( scheduler )
                                       .Subscribe ( OnStatusBarInfoChanged ) ;

        Message  = manager.LastStatusBarInfo.Message ;
        Height   = manager.LastStatusBarInfo.Height ;
        Severity = manager.LastStatusBarInfo.Severity ;

        _timer = timer ;
    }

    [ ExcludeFromCodeCoverage ]
    internal void OnElapsed ( object ? state )
    {
        Application.Current.Dispatcher.BeginInvoke ( DefaultInfoBar ) ;
    }

    internal void DefaultInfoBar ( )
    {
        if ( _manager is not { IsConnected: true } )
            return ;

        Message = Height == 0
                      ? "Can't determine desk height."
                      : $"Current desk height {Height} cm" ;

        Severity = InfoBarSeverity.Informational ;
    }

    private void OnStatusBarInfoChanged ( StatusBarInfo info )
    {
        Message  = info.Message ;
        Severity = info.Severity ;
        Height   = info.Height ;

        _timer?.Change ( TimeSpan.FromSeconds ( 10 ) ,
                         Timeout.InfiniteTimeSpan ) ;
    }
}