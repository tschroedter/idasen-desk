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
    private readonly IDisposable _deskConnected ;
    private readonly IDisposable _deskError ;
    private readonly IDisposable _deskFinished ;
    private readonly IDisposable _deskHeightChanged ;
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
        _deskHeightChanged = manager.HeightChanged
                                    .ObserveOn ( Scheduler.Default )
                                    .Subscribe ( OnDeskHeightChanged ) ;

        _deskFinished = manager.Finished
                               .ObserveOn ( Scheduler.Default )
                               .Subscribe ( OnDeskFinishedChanged ) ;

        _deskConnected = manager.Connected
                                .ObserveOn ( Scheduler.Default )
                                .Subscribe ( OnDeskConnected ) ;

        _deskError = manager.Error
                            .ObserveOn ( Scheduler.Default )
                            .Subscribe ( OnDeskError ) ;

        _timer.Elapsed += OnElapsed ;

        _timer.Interval  = TimeSpan.FromSeconds ( 10 ).TotalMilliseconds ;
        _timer.AutoReset = false ;
    }

    public void Dispose ( )
    {
        _timer.Stop ( ) ;
        _timer.Dispose ( ) ;

        _deskConnected.Dispose ( ) ;
        _deskHeightChanged.Dispose ( ) ;
        _deskFinished.Dispose ( ) ;
        _deskError.Dispose ( ) ;
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

    private void OnDeskHeightChanged ( uint height )
    {
        var heightInCm = ( uint ) Math.Round ( height / 100.0 ) ;

        Height = heightInCm ;

        Message  = $"Moving... {heightInCm} cm" ;
        Severity = InfoBarSeverity.Warning ;

        _timer.Start ( ) ;
    }

    private void OnDeskFinishedChanged ( uint height )
    {
        var heightInCm = ( uint ) Math.Round ( height / 100.0 ) ;

        Height = heightInCm ;

        Message  = "Finished!" ;
        Severity = InfoBarSeverity.Success ;

        _timer.Start ( ) ;
    }

    private void OnDeskConnected ( string message )
    {
        Message  = message;
        Severity = InfoBarSeverity.Success ;

        _timer.Start ( ) ;
    }

    private void OnDeskError ( string message )
    {
        Message  = message;
        Severity = InfoBarSeverity.Error ;

        _timer.Start ( ) ;
    }
}