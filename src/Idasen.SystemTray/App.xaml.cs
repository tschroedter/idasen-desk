using System ;
using System.Collections.Generic ;
using System.Diagnostics ;
using System.IO ;
using System.Text ;
using System.Threading.Tasks ;
using System.Windows ;
using Autofac ;
using Autofac.Core ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.Launcher ;
using Idasen.SystemTray.Interfaces ;
using Idasen.SystemTray.Settings ;
using Idasen.SystemTray.Utils ;
using Microsoft.Extensions.Configuration ;
using Serilog ;

namespace Idasen.SystemTray ;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private TaskbarIcon NotifyIcon => _provider.NotifyIcon ;

    protected override void OnStartup ( StartupEventArgs e )
    {
        base.OnStartup ( e ) ;

        UnhandledExceptionsHandler.RegisterGlobalExceptionHandling ( ) ;

        IEnumerable < IModule > otherModules = new [ ] { new SystemTrayModule ( ) } ;


        var config = GetConfiguration ( ) ;

        _container = ContainerProvider.Create ( config ,
                                                otherModules ) ;

        //create the notifyIcon (it's a resource declared in NotifyIconResources.xaml
        var factory = _container.Resolve < ITaskbarIconProviderFactory > ( ) ;
        _provider = factory.Create ( this ) ;

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
                           _container.Resolve < ILoggingSettingsManager > ( ) ,
                           _container.Resolve < Func < IDeskProvider > > ( ) ,
                           _container.Resolve < IErrorManager > ( ) ,
                           _container.Resolve < IVersionProvider > ( ) ,
                           _container.Resolve < ITaskbarIconProvider > ( ) ) ;

        // ReSharper disable once AsyncVoidLambda
        Task.Run ( new Action ( async ( ) =>
                                {
                                    await _container.Resolve<ISettingsManager>().UpgradeSettings();
                                    await model.AutoConnect ( ) ;
                                } ) ) ;
    }

    private static IConfigurationRoot GetConfiguration ( )
    {
        const string appsettingsJson = "appsettings.json" ;

        IConfigurationRoot configurationRoot = null ;

        var builder  = new StringBuilder ( ) ;
        var basePath = GetBasePath ( ) ;
        var fullPath = Path.Combine ( basePath ,
                                      appsettingsJson ) ;

        builder.AppendLine ( $"Checking if '{fullPath}' exists..." ) ;

        if ( File.Exists ( fullPath ) )
        {
            builder.AppendLine ( $"Loading settings from file '{fullPath}'..." ) ;

            configurationRoot = new ConfigurationBuilder ( ).SetBasePath ( basePath )
                                                            .AddJsonFile ( appsettingsJson )
                                                            .Build ( ) ;
        }
        else
        {
            builder.AppendLine ( $"...no, '{fullPath}' does not exists." ) ;
            builder.AppendLine ( "Using default settings..." ) ;

            configurationRoot = new ConfigurationBuilder ( ).AddJsonFile ( appsettingsJson )
                                                            .Build ( ) ;
        }

        builder.AppendLine ( "Using the following configuration:" ) ;

        builder.AppendLine ( configurationRoot.GetDebugView ( ) ) ;

        LogConfigurationSelection ( basePath ,
                                    builder ) ;

        return configurationRoot ;
    }

    private static void LogConfigurationSelection ( string        basePath ,
                                                    StringBuilder builder )
    {
        try
        {
            var logFolder = Path.Combine ( basePath ,
                                           "logs" ) ;

            if ( ! Directory.Exists ( logFolder ) )
                Directory.CreateDirectory ( logFolder ) ;

            var configLog = Path.Combine ( logFolder ,
                                           "config.log" ) ;

            if ( File.Exists ( configLog ) )
                File.Delete ( configLog ) ;

            File.WriteAllText ( configLog ,
                                builder.ToString ( ) ) ;
        }
        catch ( Exception e )
        {
            Console.WriteLine ( $"Failed to create configuration log file because {e.Message}" ) ;
        }
    }

    private static string GetBasePath ( )
    {
        using var processModule = Process.GetCurrentProcess ( ).MainModule ;

        return Path.GetDirectoryName ( processModule?.FileName ) ;
    }

    protected override void OnExit ( ExitEventArgs e )
    {
        // the icon would clean up automatically, but this is cleaner
        NotifyIcon.Dispose ( ) ;

        base.OnExit ( e ) ;
    }

    private readonly ILogger _logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                     Constants.LogFilename ) ;

    private IContainer           _container ;
    private ITaskbarIconProvider _provider ;
}