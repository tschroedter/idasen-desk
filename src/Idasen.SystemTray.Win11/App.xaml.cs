using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text ;
using System.Windows.Threading;
using Autofac ;
using Hardcodet.Wpf.TaskbarNotification;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.Launcher;
using Idasen.SystemTray.Win11.Interfaces;
using Idasen.SystemTray.Win11.Services;
using Idasen.SystemTray.Win11.Settings ;
using Idasen.SystemTray.Win11.Utils;
using Idasen.SystemTray.Win11.Utils.Exceptions;
using Idasen.SystemTray.Win11.ViewModels.Pages;
using Idasen.SystemTray.Win11.ViewModels.Windows;
using Idasen.SystemTray.Win11.Views.Pages;
using Idasen.SystemTray.Win11.Views.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wpf.Ui;

namespace Idasen.SystemTray.Win11;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App
{
    // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    private static readonly IHost Host = Microsoft.Extensions.Hosting.Host
                                                  .CreateDefaultBuilder ( )
                                                  .ConfigureAppConfiguration ( GetBasePath )
                                                  .ConfigureServices ( ( _ , services ) =>
                                                                       {
                                                                           services.AddHostedService < ApplicationHostService > ( ) ;

                                                                           // Page resolver service
                                                                           services.AddSingleton < IPageService , PageService > ( ) ;

                                                                           // Theme manipulation
                                                                           services.AddSingleton < IThemeService , ThemeService > ( ) ;

                                                                           // TaskBar manipulation
                                                                           services.AddSingleton < ITaskBarService , TaskBarService > ( ) ;

                                                                           // Service containing navigation, same as INavigationWindow... but without window
                                                                           services.AddSingleton < INavigationService , NavigationService > ( ) ;

                                                                           // Main window with navigation
                                                                           services.AddSingleton < INavigationWindow , MainWindow > ( ) ;
                                                                           services.AddSingleton < MainWindowViewModel > ( ) ;

                                                                           services.AddSingleton < DashboardPage > ( ) ;
                                                                           services.AddSingleton < DashboardViewModel > ( ) ;
                                                                           services.AddSingleton < SettingsPage > ( ) ;
                                                                           services.AddSingleton < SettingsViewModel > ( ) ;
                                                                           services.AddSingleton < IVersionProvider , VersionProvider > ( ) ;
                                                                           services.AddSingleton < ISettingsManager , SettingsManager > ( ) ;
                                                                           services.AddSingleton < ICommonApplicationData , CommonApplicationData > ( ) ;
                                                                           services.AddSingleton < ISettingsStorage , SettingsStorage > ( ) ;
                                                                       } ).Build ( ) ;

    private TaskbarIcon ? _notifyIcon ;

    private static void GetBasePath ( IConfigurationBuilder c )
    {
        c.SetBasePath ( Path.GetDirectoryName ( Assembly.GetEntryAssembly ( )!.Location ) ??
                        throw new InvalidOperationException ( "Couldn't get directory name from entry assembly" ) ) ;
    }

    private static string GetBasePath()
    {
        using var processModule = Process.GetCurrentProcess().MainModule;

        return Path.GetDirectoryName(processModule?.FileName) ?? throw new InvalidOperationException("Couldn't get directory name from entry assembly") ;
    }

    /// <summary>
    ///     Gets registered service.
    /// </summary>
    /// <typeparam name="T">Type of the service to get.</typeparam>
    /// <returns>Instance of the service or <see langword="null" />.</returns>
    public static T ? GetService < T > ( )
        where T : class
    {
        return Host.Services.GetService ( typeof ( T ) ) as T ;
    }

    /// <summary>
    ///     Occurs when the application is loading.
    /// </summary>
    private void OnStartup ( object sender , StartupEventArgs e )
    {
        UnhandledExceptionsHandler.RegisterGlobalExceptionHandling ( ) ;

        var config = GetConfiguration ( ) ;

        _container = ContainerProvider.Create ( config ) ;

        Host.Start ( ) ;

        _notifyIcon = new TaskbarIcon
        {
            Icon        = GetIconFromContent ( "Resources/cup-xl.ico" ) , // Replace with the correct relative path
            ToolTipText = "Your Application" ,
            Visibility  = Visibility.Visible
        } ;

        _notifyIcon.TrayMouseDoubleClick += NotifyIcon_DoubleClick ;

        _logger.Information ( "##### Startup..." ) ;


        var main = GetService < MainWindowViewModel > ( ) ;

        var settingsManager = GetService<ISettingsManager>() ?? throw new ArgumentNullException(nameof(ISettingsManager));

        var versionProvider = GetService<IVersionProvider>() ?? throw new ArgumentNullException(nameof(IVersionProvider));

        main.Initialize(_container.Resolve<ILogger>(),
                        _container.Resolve<Func<IDeskProvider>>(),
                        _container.Resolve<IErrorManager>());

        _logger.Information ( $"##### Idasen.SystemTray {versionProvider!.GetVersion ( )}" ) ;
    }

    private Icon GetIconFromContent ( string relativePath )
    {
        var iconPath = Path.Combine ( AppDomain.CurrentDomain.BaseDirectory , relativePath ) ;
        if ( ! File.Exists ( iconPath ) )
            throw new FileNotFoundException ( $"Icon file '{iconPath}' not found." ) ;

        return new Icon ( iconPath ) ;
    }

    private Icon GetIconFromResource ( string resourceName )
    {
        var       assembly = Assembly.GetExecutingAssembly ( ) ;
        using var stream   = assembly.GetManifestResourceStream ( resourceName ) ;
        if ( stream == null )
            throw new InvalidOperationException ( $"Resource '{resourceName}' not found." ) ;

        return new Icon ( stream ) ;
    }

    private static void NotifyIcon_DoubleClick ( object sender , RoutedEventArgs e )
    {
        var mainWindow = GetService < MainWindow > ( ) ;

        if ( mainWindow == null )
            return ;

        mainWindow.Show ( ) ;
        mainWindow.WindowState = WindowState.Normal ;
    }

    /// <summary>
    ///     Occurs when the application is closing.
    /// </summary>
    private async void OnExit ( object sender , ExitEventArgs e )
    {
        await Host.StopAsync ( ) ;

        Host.Dispose ( ) ;
    }

    /// <summary>
    ///     Occurs when an exception is thrown by an application but not handled.
    /// </summary>
    private void OnDispatcherUnhandledException ( object sender , DispatcherUnhandledExceptionEventArgs e )
    {
        // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
    }

    private readonly ILogger _logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                     Constants.LogFilename ) ;

    private IContainer ? _container ;

    private static IConfigurationRoot GetConfiguration()
    {
        const string appsettingsJson = "appsettings.json";

        IConfigurationRoot configurationRoot;

        var builder  = new StringBuilder();
        var basePath = GetBasePath();
        var fullPath = Path.Combine(basePath,
                                    appsettingsJson);

        builder.AppendLine($"Checking if '{fullPath}' exists...");

        if (File.Exists(fullPath))
        {
            builder.AppendLine($"Loading settings from file '{fullPath}'...");

            configurationRoot = new ConfigurationBuilder().SetBasePath(basePath)
                                                          .AddJsonFile(appsettingsJson)
                                                          .Build();
        }
        else
        {
            builder.AppendLine($"...no, '{fullPath}' does not exists.");
            builder.AppendLine("Using default settings...");

            configurationRoot = new ConfigurationBuilder().AddJsonFile(appsettingsJson)
                                                          .Build();
        }

        builder.AppendLine("Using the following configuration:");

        builder.AppendLine(configurationRoot.GetDebugView());

        LogConfigurationSelection(basePath,
                                  builder);

        return configurationRoot;
    }

    private static void LogConfigurationSelection(string        basePath,
                                                  StringBuilder builder)
    {
        try
        {
            var logFolder = Path.Combine(basePath,
                                         "logs");

            if (!Directory.Exists(logFolder))
                Directory.CreateDirectory(logFolder);

            var configLog = Path.Combine(logFolder,
                                         "config.log");

            if (File.Exists(configLog))
                File.Delete(configLog);

            File.WriteAllText(configLog,
                              builder.ToString());
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to create configuration log file because {e.Message}");
        }
    }
}