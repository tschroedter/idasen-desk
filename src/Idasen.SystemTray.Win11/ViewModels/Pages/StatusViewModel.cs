using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class StatusViewModel : ObservableObject , IDisposable
{
    private readonly IDisposable _statusBarInfoChanged ;
    private readonly Timer       _timer ;

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
                                       .ObserveOn ( DispatcherScheduler.Current )
                                       .Subscribe ( OnStatusBarInfoChanged ) ;

        Message  = manager.LastStatusBarInfo.Message ;
        Height   = manager.LastStatusBarInfo.Height ;
        Severity = manager.LastStatusBarInfo.Severity ;

        _timer = new Timer ( OnElapsed , 
                             null , 
                             TimeSpan.FromSeconds ( 10 ) , 
                             Timeout.InfiniteTimeSpan ) ;
    }

    public void Dispose ( )
    {
        _timer.Dispose ( ) ;
        _statusBarInfoChanged.Dispose ( ) ;
    }

    private void OnElapsed ( object ?  state )
    {
        Application.Current.Dispatcher.BeginInvoke ( DefaultInfoBar ) ;
    }

    private void DefaultInfoBar ( )
    {
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

        _timer.Change ( TimeSpan.FromSeconds ( 10 ) ,
                        Timeout.InfiniteTimeSpan ) ;
    }
}