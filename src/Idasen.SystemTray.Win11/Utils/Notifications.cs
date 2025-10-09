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
    private          bool                               _disposedValue ;

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
                           catch ( OperationCanceledException ex )
                           {
                               _logger.Warning ( ex,
                                                 "Notifications initialization canceled" ) ;
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
        Dispose ( true ) ;
        GC.SuppressFinalize ( this ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( ! _disposedValue )
        {
            if ( disposing )
            {
                // Dispose managed state (managed objects)
                _showSubscribe.Dispose ( ) ;
                _showSubject.Dispose ( ) ;
            }

            // Free unmanaged resources (if any) and set large fields to null

            _disposedValue = true ;
        }
    }

    public void Show(string title, string text, InfoBarSeverity serverity)
    {
        Show(new NotificationParameters(title,
                                        text,
                                        serverity));
    }

    public void Show ( NotificationParameters parameters )
    {
        if ( _disposedValue )
            return ;

        _showSubject.OnNext ( parameters ) ;
    }

    private void OnShow ( NotificationParameters parameters )
    {
        if ( _disposedValue )
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