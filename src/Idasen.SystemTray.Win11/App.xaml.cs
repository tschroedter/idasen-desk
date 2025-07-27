using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.IO ;
using System.IO.Abstractions ;
using System.Reactive.Concurrency ;
using System.Reflection ;
using System.Windows.Media ;
using System.Windows.Threading ;
using Autofac ;
using Autofac.Extensions.DependencyInjection ;
using AutofacSerilogIntegration ;
using Idasen.Aop ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.Launcher ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Services ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Utils.Converters ;
using Idasen.SystemTray.Win11.Utils.Exceptions ;
using Idasen.SystemTray.Win11.Utils.Icons ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Idasen.SystemTray.Win11.ViewModels.Windows ;
using Idasen.SystemTray.Win11.Views.Pages ;
using Idasen.SystemTray.Win11.Views.Windows ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.DependencyInjection ;
using Microsoft.Extensions.Hosting ;
using Wpf.Ui ;
using Wpf.Ui.Abstractions;
using Wpf.Ui.Tray.Controls ;
using ILogger = Serilog.ILogger ;

namespace Idasen.SystemTray.Win11 ;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
[ExcludeFromCodeCoverage]
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
                                                  .UseServiceProviderFactory ( new AutofacServiceProviderFactory ( ) )
                                                  .ConfigureContainer < ContainerBuilder > ( builder =>
                                                                                             {
                                                                                                 builder.RegisterLogger ( ) ;
                                                                                                 builder.RegisterModule < BluetoothLEAop > ( ) ;
                                                                                                 builder
                                                                                                    .RegisterModule < BluetoothLECoreModule > ( ) ;
                                                                                                 builder
                                                                                                    .RegisterModule < BluetoothLELinakModule > ( ) ;
                                                                                                 builder.RegisterLogger ( LoggerProvider.CreateLogger ( Constants.ApplicationName , Constants.LogFilename ) ) ;
                                                                                             } )
                                                  .ConfigureServices ( ( _ , services ) =>
                                                                       {
                                                                           services.AddHostedService < ApplicationHostService > ( ) ;

                                                                           // Page resolver service
                                                                           services.AddSingleton< INavigationViewPageProvider , PageService > ( ) ;
                                                                           services.AddSingleton < INavigationService , NavigationService > ( ) ;

                                                                           // Theme manipulation
                                                                           services.AddSingleton < IThemeService , ThemeService > ( ) ;

                                                                           // TaskBar manipulation
                                                                           services.AddSingleton < ITaskBarService , TaskBarService > ( ) ;

                                                                           // Service containing navigation, same as INavigationWindow... but without window
                                                                           services.AddSingleton < INavigationService , NavigationService > ( ) ;

                                                                           // Main window with navigation
                                                                           services.AddSingleton < INavigationWindow , IdasenDeskWindow > ( ) ;
                                                                           services.AddSingleton < IdasenDeskWindowViewModel > ( ) ;
                                                                           services.AddSingleton < HomePage > ( ) ;
                                                                           services.AddSingleton < DashboardViewModel > ( ) ;
                                                                           services.AddSingleton < StatusPage > ( ) ;
                                                                           services.AddSingleton < StatusViewModel > ( ) ;
                                                                           services.AddSingleton < SettingsPage > ( ) ;
                                                                           services.AddSingleton < SettingsViewModel > ( ) ;
                                                                           services.AddSingleton < IVersionProvider , VersionProvider > ( ) ;
                                                                           services.AddSingleton < INotifications , Notifications > ( ) ;
                                                                           services.AddSingleton < SettingsChanges > ( ) ;
                                                                           services.AddSingleton < IObserveSettingsChanges > ( GetSettingsChanged ) ;
                                                                           services.AddSingleton < INotifySettingsChanges > ( GetSettingsChanged ) ;
                                                                           services.AddSingleton < ISettingsManager , SettingsManager > ( ) ;
                                                                           services
                                                                              .AddSingleton < ILoggingSettingsManager ,
                                                                                   LoggingSettingsManager > ( ) ;
                                                                           services
                                                                              .AddSingleton < ICommonApplicationData , CommonApplicationData > ( ) ;
                                                                           services.AddSingleton < ISettingsStorage , SettingsStorage > ( ) ;
                                                                           services.AddSingleton < ITaskbarIconProvider , TaskbarIconProvider > ( ) ;
                                                                           services.AddSingleton < IUiDeskManager , UiDeskManager > ( ) ;
                                                                           services.AddSingleton < IDynamicIconCreator , DynamicIconCreator > ( ) ;
                                                                           services.AddSingleton ( _ => CreateScheduler ( ) ) ;
                                                                           services.AddSingleton ( CreateTaskbarIconProvider ) ;
                                                                           services.AddTransient < IDeviceNameConverter , DeviceNameConverter > ( ) ;
                                                                           services
                                                                              .AddTransient < IDoubleToUIntConverter , DoubleToUIntConverter > ( ) ;
                                                                           services
                                                                              .AddTransient < IStringToUIntConverter , StringToUIntConverter > ( ) ;
                                                                           services
                                                                              .AddTransient < IDeviceAddressToULongConverter ,
                                                                                   DeviceAddressToULongConverter > ( ) ;
                                                                           services.AddSingleton ( provider =>
                                                                                                      new Func < IDeskProvider > ( provider
                                                                                                         .GetRequiredService <
                                                                                                              IDeskProvider > ) ) ;
                                                                           services.AddSingleton < IFileSystem , FileSystem > ( ) ;
                                                                           services.AddTransient<IThemeSwitcher, ThemeSwitcher>();
                                                                       }).Build ( ) ;

    private readonly ILogger _logger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                     Constants.LogFilename ) ;

    private static Window CurrentWindow =>
        Current.MainWindow ?? throw new Exception ( "Can't find the main window!" ) ;

    private static SettingsChanges GetSettingsChanged ( IServiceProvider provider )
    {
        return provider.GetService < SettingsChanges > ( ) ??
               throw new InvalidOperationException ( $"Failed to resolve {nameof ( SettingsChanges )}" ) ;
    }

    private static IScheduler CreateScheduler ( )
    {
        return TaskPoolScheduler.Default ;
    }

    private static ITaskbarIconProvider CreateTaskbarIconProvider ( IServiceProvider services )
    {
        var scheduler = services.GetRequiredService < IScheduler > ( ) ;
        var creator   = services.GetRequiredService < IDynamicIconCreator > ( ) ;
        var manager   = services.GetRequiredService < ISettingsManager > ( ) ;

        return new TaskbarIconProvider ( scheduler ,
                                         creator ,
                                         manager ) ;
    }

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
        AvoidRunningTwoInstances ( ) ;

        Host.Start ( ) ;

        UnhandledExceptionsHandler.RegisterGlobalExceptionHandling ( ) ;

        _logger.Information ( "##### Startup..." ) ;

        var test = GetService < ILogger > ( ) ;
        test?.Information ( "Test" ) ;

        var notifyIcon = FindNotifyIcon ( ) ; // todo maybe we can get the icon from the view?

        var main = GetService < IdasenDeskWindowViewModel > ( ) ;

        main!.Initialize ( notifyIcon ) ;

        var settings = GetService < SettingsViewModel > ( ) ;

        settings!.Initialize ( CancellationToken.None ) ;

        var versionProvider = GetVersionProvider ( ) ;

        _logger.Information ( $"##### Idasen.SystemTray {versionProvider.GetVersion ( )}" ) ;
    }

    private void AvoidRunningTwoInstances ( )
    {
        var location                 = Assembly.GetEntryAssembly ( )?.Location ?? throw new Exception ( "Can't get entry assembly!" ) ;
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension ( location ) ;

        if ( Process.GetProcessesByName ( fileNameWithoutExtension ).Length <= 1 )
            return ;

        _logger.Information ( "##### Application already running!" ) ;

        Process.GetCurrentProcess ( ).Kill ( ) ;
    }

    private static IVersionProvider GetVersionProvider ( )
    {
        return GetService < IVersionProvider > ( ) ?? throw new ArgumentNullException ( nameof ( IVersionProvider ) ) ;
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
        _logger.Error ( e.Exception ,
                         "Unhandled exception" ) ;
    }

    private static NotifyIcon FindNotifyIcon ( )
    {
        if ( ! CurrentWindow.CheckAccess ( ) )
        {
            CurrentWindow.Dispatcher.BeginInvoke ( new Action ( ( ) => FindNotifyIcon ( ) ) ) ;
        }

        var notifyIcons = FindVisualChildren < NotifyIcon > ( CurrentWindow ) ;

        return notifyIcons.FirstOrDefault ( ) ?? throw new Exception ( "Can't find the main notify icon!" ) ;
    }

    public static IEnumerable < T > FindVisualChildren < T > ( DependencyObject parent )
        where T : DependencyObject
    {
        var childrenCount = VisualTreeHelper.GetChildrenCount ( parent ) ;

        for ( var i = 0 ; i < childrenCount ; i ++ )
        {
            var child = VisualTreeHelper.GetChild ( parent ,
                                                    i ) ;

            if ( child is T childType )
            {
                yield return childType ;
            }

            foreach ( var other in FindVisualChildren < T > ( child ) )
            {
                yield return other ;
            }
        }
    }
}