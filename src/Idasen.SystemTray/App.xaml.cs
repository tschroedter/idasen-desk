using System ;
using System.Collections.Generic ;
using System.IO ;
using System.Threading.Tasks ;
using System.Windows ;
using Autofac ;
using Autofac.Core ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.Launcher ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Utils ;
using Microsoft.Extensions.Configuration ;
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

            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json");

            _container = ContainerProvider.Create(builder.Build(),
                                                  otherModules);

            //create the notifyIcon (it's a resource declared in NotifyIconResources.xaml
            var factory = _container.Resolve < ITaskbarIconProviderFactory> ( ) ;
            _provider = factory.Create(this);

            if ( NotifyIcon == null )
                throw new ArgumentException ( "Can't find resource: NotifyIcon" ,
                                              nameof ( NotifyIcon ) ) ;

            if ( ! ( NotifyIcon?.DataContext is NotifyIconViewModel model ) )
                throw new ArgumentException ( "Can't find DataContext: NotifyIconViewModel" ,
                                              nameof ( model ) ) ;

            _logger.Information ( "##### Startup..." ) ;

            var versionProvider = _container.Resolve < IVersionProvider > ( ) ;

            _logger.Information ( $"##### Idasen.SystemTray {versionProvider.GetVersion ( )}" ) ;

            model.Initialize ( _container.Resolve < ILogger > ( ) ,
                               _container.Resolve < ISettingsManager > ( ) ,
                               _container.Resolve < Func < IDeskProvider > > ( ) ,
                               _container.Resolve < IErrorManager > ( ) ,
                               _container.Resolve < IVersionProvider > ( ) ,
                               _container.Resolve < ITaskbarIconProvider > ( ) ) ;

            // ReSharper disable once AsyncVoidLambda
            Task.Run ( new Action ( async ( ) => await model.AutoConnect ( ) ) ) ;
        }

        protected override void OnExit ( ExitEventArgs e )
        {
            // the icon would clean up automatically, but this is cleaner
            NotifyIcon.Dispose ( ) ;

            base.OnExit ( e ) ;
        }

        private readonly ILogger _logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                         Constants.LogFilename ) ;

        private IContainer          _container ;
        private TaskbarIcon         NotifyIcon => _provider.NotifyIcon ;
        private ITaskbarIconProvider _provider ;
    }
}