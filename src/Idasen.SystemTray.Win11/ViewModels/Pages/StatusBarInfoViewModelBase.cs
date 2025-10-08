using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class StatusBarInfoViewModelBase : ObservableObject , IDisposable
{
    private readonly IDisposable _statusBarInfoChanged ;
    private readonly ITimer      _timer ;

    private readonly IUiDeskManager _uiDeskManager ;

    [ ObservableProperty ] private uint _height ;

    [ ObservableProperty ] private string _message = "Unknown" ;

    [ ObservableProperty ] private InfoBarSeverity _severity = InfoBarSeverity.Informational ;

    [ ObservableProperty ] private string _title = "Desk Status" ;

    public StatusBarInfoViewModelBase (
        IUiDeskManager                                                   uiDeskManager ,
        IScheduler                                                       scheduler ,
        Func < TimerCallback , object ? , TimeSpan , TimeSpan , ITimer > timerFactory )
    {
        _uiDeskManager = uiDeskManager ;
        Scheduler      = scheduler ;

        // Subscribe to changes on provided scheduler
        _statusBarInfoChanged = uiDeskManager.StatusBarInfoChanged
                                             .ObserveOn ( Scheduler )
                                             .Subscribe ( OnStatusBarInfoChangedInternal ) ;

        // Create timer; fire once after 10s, no periodic interval
        _timer = timerFactory (
                               OnElapsed ,
                               null ,
                               TimeSpan.FromSeconds ( 10 ) ,
                               Timeout.InfiniteTimeSpan ) ;
    }

    protected IScheduler Scheduler { get ; }

    public virtual void Dispose ( )
    {
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( disposing )
        {
            _timer.Dispose ( ) ;
            _statusBarInfoChanged.Dispose ( ) ;
        }
    }

    private void OnStatusBarInfoChangedInternal ( StatusBarInfo info )
    {
        // Update properties
        Message  = info.Message ;
        Severity = info.Severity ;
        Height   = info.Height ;

        RestartTimer ( ) ;
        OnStatusUpdated ( info ) ;
    }

    // Hook for derived classes if they need additional behavior
    protected virtual void OnStatusUpdated ( StatusBarInfo info )
    {
    }

    protected void RestartTimer ( )
    {
        _timer.Change ( TimeSpan.FromSeconds ( 10 ) ,
                        Timeout.InfiniteTimeSpan ) ;
    }

    [ ExcludeFromCodeCoverage ]
    internal void OnElapsed ( object ? state )
    {
        Application.Current.Dispatcher.BeginInvoke ( DefaultInfoBar ) ;
    }

    internal void DefaultInfoBar ( )
    {
        if ( _uiDeskManager is not { IsConnected: true } )
            return ;

        Message = Height == 0
                      ? "Can't determine desk height."
                      : $"Current desk height {Height} cm" ;

        Severity = InfoBarSeverity.Informational ;
    }
}