using System.Windows.Threading ;
using Autofac ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Settings ;
using Microsoft.Toolkit.Uwp.Notifications ;
using Serilog ;
using Wpf.Ui.Tray.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public class Notifications : INotifications
{
    private readonly ISettingsManager _manager ;
    private          ILogger ?        _logger ;

    public Notifications ( ISettingsManager manager )
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;

        _manager = manager ;
    }

    public void Show ( string     title ,
                                   string     text ,
                                   Visibility visibilityBulbGreen  = Visibility.Hidden ,
                                   Visibility visibilityBulbYellow = Visibility.Hidden ,
                                   Visibility visibilityBulbRed    = Visibility.Hidden )
    {
        if ( _manager is { CurrentSettings.NotificationsEnabled: false } )
        {
            _logger?.Information ( $"Notifications are disabled. " +
                                   $"Title = '{title}' "           +
                                   $"Text = '{text}'" ) ;

            return ;
        }

        if ( ! Dispatcher.CurrentDispatcher.CheckAccess ( ) )
        {
            _logger?.Debug ( "Dispatching call on UI thread" ) ;

            Dispatcher.CurrentDispatcher.BeginInvoke ( new Action ( ( ) => Show ( title ,
                                                                                              text ,
                                                                                              visibilityBulbGreen ,
                                                                                              visibilityBulbYellow ,
                                                                                              visibilityBulbRed ) ) ) ;

            return ;
        }

        _logger?.Debug ( $"Title = '{title}', "                              +
                         $"Text = '{text}', "                                +
                         $"visibilityBulbGreen = '{visibilityBulbGreen}', "  +
                         $"visibilityBulbYellow = '{visibilityBulbYellow}' " +
                         $"visibilityBulbRed = '{visibilityBulbRed}'" ) ;

        // Requires Microsoft.Toolkit.Uwp.Notifications NuGet package version 7.0 or greater
        var builder = new ToastContentBuilder ( ) ;
        builder.AddText ( title ) ;
        builder.AddText ( text ) ; // todo image balloon

        // Not seeing the Show() method? Make sure you have version 7.0, and if you're using .NET 6 (or later), then your TFM must be net6.0-windows10.0.17763.0 or greater
        // Try running this code and you should see the notification appear!
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
}