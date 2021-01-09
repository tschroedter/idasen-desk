using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;
using System.Windows ;
using Autofac ;
using Autofac.Core ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.Launcher ;
using Idasen.SystemTray.Interfaces ;
using Serilog ;

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

            IEnumerable < IModule > otherModules = new [ ] { new SystemTrayModule ( ) } ;

            _container = ContainerProvider.Create ( "Idasen.SystemTray" ,
                                                    "Idasen.SystemTray.log" ,
                                                    otherModules ) ;

            _logger = _container.Resolve < ILogger > ( ) ;

            UnhandledExceptionsHandler.RegisterGlobalExceptionHandling ( _logger ) ;

            //create the notifyIcon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = ( TaskbarIcon ) FindResource ( "NotifyIcon" ) ;

            if ( _notifyIcon == null )
                throw new ArgumentException ( "Can't find resource: NotifyIcon" ,
                                              nameof ( _notifyIcon ) ) ;

            var model = _notifyIcon?.DataContext as NotifyIconViewModel ;

            if ( model == null )
                throw new ArgumentException ( "Can't find DataContext: NotifyIconViewModel" ,
                                              nameof ( model ) ) ;
            model.Initialize ( _logger ,
                               _container.Resolve < ISettingsManager > ( ) ,
                               _container.Resolve < IDeskProvider > ( ) ) ;

            Task.Run ( new Action ( async ( ) => await model.AutoConnect ( ) ) ) ;
        }

        protected override void OnExit ( ExitEventArgs e )
        {
            // the icon would clean up automatically, but this is cleaner
            _notifyIcon.Dispose ( ) ;

            base.OnExit ( e ) ;
        }

        private IContainer _container ;
        private ILogger    _logger ;

        private TaskbarIcon _notifyIcon ;
    }
}