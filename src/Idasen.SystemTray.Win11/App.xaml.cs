using System.Drawing;
using System.IO;
using System.Reactive.Concurrency ;
using System.Reflection;
using System.Windows.Media ;
using System.Windows.Shell ;
using System.Windows.Threading;
using Autofac ;
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
using JetBrains.Annotations ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wpf.Ui;
using Wpf.Ui.Tray.Controls ;

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
                                                                           services.AddSingleton < HomePage > ( ) ;
                                                                           services.AddSingleton < DashboardViewModel > ( ) ;
                                                                           services.AddSingleton < StatusPage > ( ) ;
                                                                           services.AddSingleton < StatusViewModel > ( ) ;
                                                                           services.AddSingleton < SettingsPage > ( ) ;
                                                                           services.AddSingleton < SettingsViewModel > ( ) ;
                                                                           services.AddSingleton < IVersionProvider , VersionProvider > ( ) ;
                                                                           services.AddSingleton < INotifications , Notifications > ( ) ;
                                                                           services.AddSingleton < ISettingsManager , SettingsManager > ( ) ;
                                                                           services.AddSingleton < ICommonApplicationData , CommonApplicationData > ( ) ;
                                                                           services.AddSingleton < ISettingsStorage , SettingsStorage > ( ) ;
                                                                           services.AddSingleton < ITaskbarIconProvider , TaskbarIconProvider > ( ) ;
                                                                           services.AddSingleton < IUiDeskManager , UiDeskManager > ( ) ;
                                                                           services.AddSingleton < IDynamicIconCreator , DynamicIconCreator > ( ) ;
                                                                           services.AddSingleton < IIdasenConfigurationProvider , IdasenConfigurationProvider > ( ) ;
                                                                           services.AddSingleton ( _ => CreateScheduler ( ) ) ;
                                                                           services.AddSingleton ( CreateTaskbarIconProvider ) ;
                                                                       } ).Build ( ) ;

    private static IScheduler CreateScheduler()
    {
        return TaskPoolScheduler.Default;
    }

    private static ITaskbarIconProvider CreateTaskbarIconProvider(IServiceProvider services)
    {
        var scheduler = services.GetRequiredService<IScheduler>();
        var creator   = services.GetRequiredService<IDynamicIconCreator>();

        return new TaskbarIconProvider(scheduler,
                                       creator);
    }

    //private TaskbarIcon ? _notifyIcon ;

    private static void GetBasePath ( IConfigurationBuilder c )
    {
        c.SetBasePath ( Path.GetDirectoryName ( Assembly.GetEntryAssembly ( )!.Location ) ??
                        throw new InvalidOperationException ( "Couldn't get directory name from entry assembly" ) ) ;
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

        Host.Start ( ) ;

        _logger.Information ( "##### Startup..." ) ;

        var configurationProvider = GetService<IIdasenConfigurationProvider>(); 

        _container = ContainerProvider.Create(configurationProvider!.GetConfiguration (  )); // todo only use one container

        var notifyIcon = FindNotifyIcon();

        var main = GetService < MainWindowViewModel > ( ) ;

        main!.Initialize(_container, notifyIcon);

        var settings = GetService<SettingsViewModel>();

        settings!.Initialize();

        var versionProvider = GetVersionProvider ( );

        _logger.Information ( $"##### Idasen.SystemTray {versionProvider.GetVersion ( )}" ) ;
    }

    private static IVersionProvider GetVersionProvider() =>
       GetService<IVersionProvider>() ?? throw new ArgumentNullException(nameof(IVersionProvider)) ;

    private Icon GetIconFromContent ( string relativePath )
    {
        var iconPath = Path.Combine ( AppDomain.CurrentDomain.BaseDirectory , relativePath ) ;
        if ( ! File.Exists ( iconPath ) )
            throw new FileNotFoundException ( $"Icon file '{iconPath}' not found." ) ;

        return new Icon ( iconPath ) ;
    }

    [UsedImplicitly]
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
        var mainWindow = GetService < INavigationWindow > ( ) ;

        if ( mainWindow == null )
            return ;

        mainWindow.ShowWindow (  );
        //mainWindow.WindowState = WindowState.Normal ;
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

        private static NotifyIcon FindNotifyIcon()
    {
        if (Application.Current.MainWindow != null && !Application.Current.MainWindow.CheckAccess())
        {
            Application.Current.MainWindow.Dispatcher.BeginInvoke(new Action(() => FindNotifyIcon()));
        }

        IEnumerable < NotifyIcon > notifyIcons = FindVisualChildren<NotifyIcon> (Application.Current.MainWindow) ;

        return notifyIcons.FirstOrDefault();
    }

    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject parent)
        where T : DependencyObject
    {
        int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
        for (int i = 0; i < childrenCount; i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            var childType = child as T;
            if (childType != null)
            {
                yield return (T)child;
            }

            foreach (var other in FindVisualChildren<T>(child))
            {
                yield return other;
            }
        }
    }
}
