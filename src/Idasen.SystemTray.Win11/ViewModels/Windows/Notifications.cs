using System.Reactive.Subjects ;
using System.Windows.Threading ;
using Autofac ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Settings ;
using Microsoft.Toolkit.Uwp.Notifications ;
using Serilog ;
using Windows.UI.Notifications;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public class Notifications : INotifications
{
    private readonly ISettingsManager                   _manager ;
    private          ILogger ?                          _logger ;
    private readonly Subject < NotificationParameters > _showSubject ;
    private readonly IDisposable                        _showSubscribe ;

    public Notifications ( ISettingsManager manager )
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;

        _manager = manager ;
        _showSubject = new Subject<NotificationParameters>();
        _showSubscribe = _showSubject.Subscribe(OnShow);
    }

    public void Show(string     title,
                     string     text,
                     Visibility visibilityBulbGreen  = Visibility.Hidden,
                     Visibility visibilityBulbYellow = Visibility.Hidden,
                     Visibility visibilityBulbRed    = Visibility.Hidden)
    {
        _showSubject.OnNext(new NotificationParameters(title,
                                                       text,
                                                       visibilityBulbGreen,
                                                       visibilityBulbYellow,
                                                       visibilityBulbRed));
    }

    public void Show (NotificationParameters parameters )
    {
        _showSubject.OnNext ( parameters ) ;
    }

    private void OnShow (NotificationParameters parameters )
    {
        if ( _manager is { CurrentSettings.NotificationsEnabled: false } )
        {
            _logger?.Information ( $"Notifications are disabled. " +
                                   $"Title = '{parameters.Title}' " +
                                   $"Text = '{parameters.Text}'" ) ;

            return ;
        }

        if ( ! Dispatcher.CurrentDispatcher.CheckAccess ( ) )
        {
            _logger?.Debug ( "Dispatching call on UI thread" ) ;

            Dispatcher.CurrentDispatcher.BeginInvoke ( new Action ( ( ) => Show ( parameters) ) ) ;

            return ;
        }

        _logger?.Debug ( $"Parameters = {parameters}") ;

        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        var builder = new ToastContentBuilder ( ) ;
        builder.AddText ( parameters.Title ) ;
        builder.AddText ( parameters.Text ) ; // todo image balloon

        // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
        // Try running this code, and you should see the notification appear!
        builder.Show ( ) ;
    }

    public INotifications Initialize ( IContainer container , NotifyIcon notifyIcon )
    {
        Guard.ArgumentNotNull ( container ,
                                nameof ( container ) ) ;

        _logger = container.Resolve < ILogger > ( ) ;

        _logger?.Debug ( "Notifications initializing..." ) ;

        return this ;
    }

    public void Dispose ( )
    {
        _showSubject.Dispose ( ) ;
        _showSubscribe.Dispose ( ) ;
    }
}