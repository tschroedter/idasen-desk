using System.Windows ;
using Hardcodet.Wpf.TaskbarNotification ;

namespace Idasen.SystemTray
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override void OnStartup ( StartupEventArgs e )
        {
            base.OnStartup ( e ) ;

            //create the notifyIcon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = ( TaskbarIcon ) FindResource ( "NotifyIcon" ) ;
        }

        protected override void OnExit ( ExitEventArgs e )
        {
            _notifyIcon.Dispose ( ) ; //the icon would clean up automatically, but this is cleaner
            base.OnExit ( e ) ;
        }

        private TaskbarIcon _notifyIcon ;
    }
}