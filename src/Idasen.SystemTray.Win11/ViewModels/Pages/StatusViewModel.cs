using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Timers ;
using System.Windows.Threading ;
using Idasen.SystemTray.Win11.ViewModels.Windows ;
using Wpf.Ui.Controls ;
using Timer = System.Timers.Timer ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class StatusViewModel : ObservableObject , IDisposable
{
    private readonly IDisposable _statusBarInfoChanged ;
    private readonly Timer       _timer = new( ) ;

    [ ObservableProperty ]
    private uint _height ;

    [ ObservableProperty ]
    private string _message = "Unknown" ;

    [ ObservableProperty ]
    private InfoBarSeverity _severity = InfoBarSeverity.Informational ;


    [ ObservableProperty ]
    private string _title = "Desk Status" ;

    public StatusViewModel ( IUiDeskManager manager )
    {
        _statusBarInfoChanged = manager.StatusBarInfoChanged
                                       .ObserveOn ( Scheduler.Default )
                                       .Subscribe ( OnStatusBarInfoChanged ) ;

        Message          =  manager.LastStatusBarInfo.Message ;
        Height           =  manager.LastStatusBarInfo.Height;
        Severity         =  manager.LastStatusBarInfo.Severity;

        _timer.Elapsed   += OnElapsed;
        _timer.Interval  =  TimeSpan.FromSeconds(10).TotalMilliseconds;
        _timer.AutoReset =  false;
        _timer.Start ( ) ;
    }

    public void Dispose ( )
    {
        _timer.Elapsed -= OnElapsed ;
        _timer.Stop ( ) ;
        _timer.Dispose ( ) ;

        _statusBarInfoChanged.Dispose ( ) ;
    }

    private void OnElapsed ( object ? source , ElapsedEventArgs e )
    {
        DefaultInfoBar ( ) ;
    }

    private void DefaultInfoBar ( )
    {
        if ( ! Dispatcher.CurrentDispatcher.CheckAccess ( ) )
        {
            Dispatcher.CurrentDispatcher.Invoke ( DefaultInfoBar ) ;
            return ;
        }

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

        _timer.Start ( ) ;
    }
}