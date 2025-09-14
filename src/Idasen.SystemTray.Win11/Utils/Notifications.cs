using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class Notifications : INotifications
{
    private readonly ILogger                            _logger ;
    private readonly ISettingsManager                   _manager ;
    private readonly Subject < NotificationParameters > _showSubject ;
    private readonly IDisposable                        _showSubscribe ;
    private readonly IToastService                      _toast ;
    private readonly IVersionProvider                   _version ;
    private volatile bool                               _disposed ;

    public Notifications ( ILogger          logger ,
                           ISettingsManager manager ,
                           IVersionProvider version ,
                           IToastService    toast )
    {
        _logger        = logger ;
        _manager       = manager ;
        _version       = version ;
        _toast         = toast ;
        _showSubject   = new Subject < NotificationParameters > ( ) ;
        _showSubscribe = _showSubject.Subscribe ( OnShow ) ;
    }

    public void Show ( string title , string text , InfoBarSeverity severity )
    {
        Show ( new NotificationParameters ( title ,
                                            text ,
                                            severity ) ) ;
    }

    public INotifications Initialize ( NotifyIcon        notifyIcon ,
                                       CancellationToken token )
    {
        _logger.Debug ( "Notifications initializing..." ) ;

        _ = Task.Run ( async ( ) =>
                       {
                           try
                           {
                               await _manager.LoadAsync ( token ).ConfigureAwait ( false ) ;

                               Show ( $"Idasen System Tray {_version.GetVersion ( )}" ,
                                      "Running..." ,
                                      InfoBarSeverity.Informational ) ;
                           }
                           catch ( OperationCanceledException )
                           {
                               _logger.Information ( "Notifications initialization canceled" ) ;
                           }
                           catch ( Exception ex )
                           {
                               _logger.Error ( ex ,
                                               "Failed to initialize notifications" ) ;
                           }
                       } ,
                       token ) ;

        return this ;
    }

    public void Dispose ( )
    {
        _disposed = true ;
        _showSubscribe.Dispose ( ) ;
        _showSubject.Dispose ( ) ;
    }

    public void Show ( NotificationParameters parameters )
    {
        if ( _disposed )
            return ;

        _showSubject.OnNext ( parameters ) ;
    }

    private void OnShow ( NotificationParameters parameters )
    {
        if ( _disposed )
            return ;

        if ( ! _manager.CurrentSettings.DeviceSettings.NotificationsEnabled )
        {
            _logger.Information ( "Notifications are disabled. {Parameters}" ,
                                  parameters ) ;
            return ;
        }

        var appDispatcher = Application.Current?.Dispatcher ;

        if ( appDispatcher != null &&
             ! appDispatcher.CheckAccess ( ) )
        {
            _logger.Debug ( "Dispatching call on UI thread" ) ;
            appDispatcher.BeginInvoke ( new Action ( ( ) => Show ( parameters ) ) ) ;
            return ;
        }

        _logger.Debug ( "Parameters = {Parameters}" ,
                        parameters ) ;

        _toast.Show ( parameters ) ;
    }
}