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
using Idasen.SystemTray.Utils ;
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

            UnhandledExceptionsHandler.RegisterGlobalExceptionHandling ( ) ;

            IEnumerable < IModule > otherModules = new [ ] { new SystemTrayModule ( ) } ;

            _container = ContainerProvider.Create ( Constants.ApplicationName ,
                                                    Constants.LogFilename ,
                                                    otherModules ) ;

            //create the notifyIcon (it's a resource declared in NotifyIconResources.xaml
            _notifyIcon = ( TaskbarIcon ) FindResource ( "NotifyIcon" ) ;

            if ( _notifyIcon == null )
                throw new ArgumentException ( "Can't find resource: NotifyIcon" ,
                                              nameof ( _notifyIcon ) ) ;

            if ( ! ( _notifyIcon?.DataContext is NotifyIconViewModel model ) )
                throw new ArgumentException ( "Can't find DataContext: NotifyIconViewModel" ,
                                              nameof ( model ) ) ;

            _logger.Information ( "##### Startup..." ) ;

            model.Initialize ( _container.Resolve < ILogger > ( ) ,
                               _container.Resolve < ISettingsManager > ( ) ,
                               _container.Resolve < IDeskProvider > ( ) ,
                               _container.Resolve < IErrorManager > ( ) ) ;

            Task.Run ( new Action ( async ( ) => await model.AutoConnect ( ) ) ) ;
        }

        protected override void OnExit ( ExitEventArgs e )
        {
            // the icon would clean up automatically, but this is cleaner
            _notifyIcon.Dispose ( ) ;

            base.OnExit ( e ) ;
        }

        private readonly ILogger _logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                         Constants.LogFilename ) ;

        private IContainer  _container ;
        private TaskbarIcon _notifyIcon ;
    }
}