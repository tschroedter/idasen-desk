using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class DeskNotificationManager : IDeskNotificationManager
{
    private readonly IDeskConnectionManager _deskConnectionManager ;
    private readonly IErrorManager          _errorManager ;
    private readonly ILogger                _logger ;
    private readonly INotifications         _notifications ;
    private readonly IScheduler             _scheduler ;
    private readonly ISettingsManager       _settingsManager ;
    private readonly IStatusBarManager      _statusBarManager ;
    private          bool                   _disposed ;

    private IDisposable ? _errorChangedSubscription ;

    public DeskNotificationManager (
        ILogger                logger ,
        INotifications         notifications ,
        IErrorManager          errorManager ,
        ISettingsManager       settingsManager ,
        IStatusBarManager      statusBarManager ,
        IScheduler             scheduler ,
        IDeskConnectionManager deskConnectionManager )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;
        ArgumentNullException.ThrowIfNull ( notifications ) ;
        ArgumentNullException.ThrowIfNull ( errorManager ) ;
        ArgumentNullException.ThrowIfNull ( settingsManager ) ;
        ArgumentNullException.ThrowIfNull ( statusBarManager ) ;
        ArgumentNullException.ThrowIfNull ( scheduler ) ;
        ArgumentNullException.ThrowIfNull ( deskConnectionManager ) ;

        _logger                = logger ;
        _notifications         = notifications ;
        _errorManager          = errorManager ;
        _settingsManager       = settingsManager ;
        _statusBarManager      = statusBarManager ;
        _scheduler             = scheduler ;
        _deskConnectionManager = deskConnectionManager ;
    }

    public IObservable < StatusBarInfo > StatusBarInfoChanged => _statusBarManager.StatusBarInfoChanged ;

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        try
        {
            _errorChangedSubscription?.Dispose ( ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                              "Failed to dispose {ResourceName}" ,
                              nameof ( _errorChangedSubscription ) ) ;
        }

        _errorChangedSubscription = null ;
    }

    public void Initialize ( NotifyIcon notifyIcon , CancellationToken token )
    {
        _errorChangedSubscription = _errorManager.ErrorChanged
                                                 .ObserveOn ( _scheduler )
                                                 .Subscribe ( OnErrorChanged ) ;

        _notifications.Initialize ( notifyIcon ,
                                    token ) ;
    }

    public void ShowNotification ( string title , string message , InfoBarSeverity severity )
    {
        _notifications.Show ( title ,
                              message ,
                              severity ) ;
    }

    public void ShowStatusUpdate ( uint            height ,
                                   string          title ,
                                   string          message ,
                                   InfoBarSeverity severity )
    {
        _logger.Debug ( "Status update Height={Height} Title={Title} Message={Message} Severity={Severity}" ,
                        height ,
                        title ,
                        message ,
                        severity ) ;

        if ( height == 0 )
            height = _settingsManager.CurrentSettings.HeightSettings.LastKnownDeskHeight ;

        var info = new StatusBarInfo ( title ,
                                       height ,
                                       message ,
                                       severity ) ;

        _statusBarManager.UpdateStatus ( info ) ;

        _notifications.Show ( title ,
                              message ,
                              severity ) ;
    }

    private void OnErrorChanged ( IErrorDetails details )
    {
        var deviceName = _deskConnectionManager.CurrentDesk?.DeviceName ?? "Unknown" ;
        var msg        = $"[{deviceName}] {details.Message}" ;

        _logger.Error ( "Desk error: {Message}" ,
                        msg ) ;

        ShowStatusUpdate ( 0 ,
                           "Error" ,
                           msg ,
                           InfoBarSeverity.Error ) ;
    }
}
