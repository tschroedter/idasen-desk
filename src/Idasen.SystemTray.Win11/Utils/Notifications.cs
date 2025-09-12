using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Subjects ;
using System.Windows ;
using System.Windows.Threading ;
using Idasen.SystemTray.Win11.Interfaces ;
using Microsoft.Toolkit.Uwp.Notifications ;
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
    private readonly IVersionProvider                   _version ;

    public Notifications ( ILogger          logger ,
                           ISettingsManager manager ,
                           IVersionProvider version )
    {
        _logger        = logger ;
        _manager       = manager ;
        _version       = version ;
        _showSubject   = new Subject < NotificationParameters > ( ) ;
        _showSubscribe = _showSubject.Subscribe ( OnShow ) ;
    }

    public void Show ( string title , string text , InfoBarSeverity severity )
    {
        Show ( new NotificationParameters ( title ,
                                            text ,
                                            severity ) ) ;
    }

    public INotifications Initialize ( NotifyIcon notifyIcon ,
                                       CancellationToken token)
    {
        _logger.Debug ( "Notifications initializing..." ) ;

        Task.Run ( async ( ) =>
                   {
                       await _manager.LoadAsync ( token ) ;
                       Show ( $"Idasen System Tray {_version.GetVersion ( )}" ,
                              "Running..." ,
                              InfoBarSeverity.Informational ) ;
                   } ,
                   token ) ;

        return this ;
    }

    public void Dispose ( )
    {
        _showSubscribe.Dispose ( ) ;
        _showSubject.Dispose ( ) ;
    }

    public void Show ( NotificationParameters parameters )
    {
        _showSubject.OnNext ( parameters ) ;
    }

    private void OnShow ( NotificationParameters parameters )
    {
        if ( ! _manager.CurrentSettings.DeviceSettings.NotificationsEnabled )
        {
            _logger.Information ( "Notifications are disabled. {Parameters}" ,
                                  parameters ) ;
            return ;
        }

        var appDispatcher = Application.Current?.Dispatcher ;

        if ( appDispatcher != null && ! appDispatcher.CheckAccess ( ) )
        {
            _logger.Debug ( "Dispatching call on UI thread" ) ;
            appDispatcher.BeginInvoke ( new Action ( ( ) => Show ( parameters ) ) ) ;
            return ;
        }

        _logger.Debug ( "Parameters = {Parameters}" ,
                        parameters ) ;

        var builder = new ToastContentBuilder ( )
                     .AddText ( parameters.Title )
                     .AddText ( parameters.Text )
                     .SetToastDuration ( ToastDuration.Short ) ;

        builder.Show ( ) ;
    }
}