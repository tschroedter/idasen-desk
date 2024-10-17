using System.Reactive.Subjects ;
using System.Windows.Threading ;
using Autofac ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Toolkit.Uwp.Notifications ;
using Serilog ;
using Wpf.Ui.Controls ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.Utils ;

public class Notifications : INotifications
{
    private readonly ISettingsManager                   _manager ;
    private readonly Subject < NotificationParameters > _showSubject ;
    private readonly IDisposable                        _showSubscribe ;
    private readonly IVersionProvider                   _version ;
    private          ILogger ?                          _logger ;

    public Notifications ( ISettingsManager manager ,
                           IVersionProvider version )
    {
        _manager       = manager ;
        _version       = version ;
        _showSubject   = new Subject < NotificationParameters > ( ) ;
        _showSubscribe = _showSubject.Subscribe ( OnShow ) ;
    }

    public void Show ( string     title ,
                       string     text ,
                       InfoBarSeverity serverity)
    {
        var parameters = new NotificationParameters ( title ,
                                                      text ,
                                                      serverity ) ;
        Show( parameters ) ;
    }

    public void Show ( NotificationParameters parameters)  // todo maybe just one show method
    {
        if (_manager is { CurrentSettings.DeviceSettings.NotificationsEnabled: false })
        {
            _logger?.Information($"Notifications are disabled. " +
                                 $"{nameof(parameters)}: {parameters}");

            return;
        }

        _showSubject.OnNext ( parameters ) ;
    }

    public INotifications Initialize ( IContainer container , NotifyIcon notifyIcon )
    {
        Guard.ArgumentNotNull ( container ,
                                nameof ( container ) ) ;

        _logger = container.Resolve < ILogger > ( ) ;

        _logger?.Debug ( "Notifications initializing..." ) ;

        Task.Run ( async ( ) =>
                   {
                       await _manager.LoadAsync ( ) ;

                       Show ( $"Idasen System Tray {_version.GetVersion ( )}" ,
                              "Running..." ,
                              InfoBarSeverity.Informational ) ;
                   } ) ;

        return this ;
    }

    public void Dispose ( )
    {
        _showSubject.Dispose ( ) ;
        _showSubscribe.Dispose ( ) ;
    }

    private void OnShow ( NotificationParameters parameters )
    {
        if ( _manager is { CurrentSettings.DeviceSettings.NotificationsEnabled: false } )
        {
            _logger?.Information ( $"Notifications are disabled. " +
                                   $"{nameof ( parameters )}: {parameters}" ) ;

            return ;
        }

        if ( ! Dispatcher.CurrentDispatcher.CheckAccess ( ) )
        {
            _logger?.Debug ( "Dispatching call on UI thread" ) ;

            Dispatcher.CurrentDispatcher.BeginInvoke ( new Action ( ( ) => Show ( parameters ) ) ) ;

            return ;
        }

        _logger?.Debug ( $"Parameters = {parameters}" ) ;

        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        var builder = new ToastContentBuilder ( ) ;
        builder.AddText ( parameters.Title ) ;
        builder.AddText ( parameters.Text ) ; // todo image balloon
        builder.SetToastDuration ( ToastDuration.Short ) ;

        // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
        // Try running this code, and you should see the notification appear!
        builder.Show ( ) ;
    }
}