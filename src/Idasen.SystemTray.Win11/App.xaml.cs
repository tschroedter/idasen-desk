using System.Diagnostics.CodeAnalysis ;
using System.IO ;
using System.IO.Abstractions ;
using System.Reactive.Concurrency ;
using System.Reflection ;
using System.Windows.Media ;
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
using Serilog ;
using Wpf.Ui ;
using Wpf.Ui.Abstractions ;
using Wpf.Ui.Tray.Controls ;
using ILogger = Serilog.ILogger ;

namespace Idasen.SystemTray.Win11 ;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
[ ExcludeFromCodeCoverage ]
public partial class App
{
    // Configure a single Serilog logger instance for the entire app
    private static readonly ILogger AppLogger = LoggerProvider.CreateLogger ( Constants.ApplicationName ,
                                                                              Constants.LogFilename ) ;

    static App ( )
    {
        Log.Logger = AppLogger ;
    }

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
                                                                                                 // Register Serilog logger instance for DI
                                                                                                 builder.RegisterLogger ( AppLogger ) ;
                                                                                                 builder.RegisterModule < BluetoothLEAop > ( ) ;
                                                                                                 builder.RegisterModule < BluetoothLECoreModule > ( ) ;
                                                                                                 builder.RegisterModule < BluetoothLELinakModule > ( ) ;
                                                                                             } )
                                                  .ConfigureServices ( ( _ , services ) =>
                                                                       {
                                                                           services.AddHostedService < ApplicationHostService > ( ) ;

                                                                           // Page resolver service
                                                                           services.AddSingleton < INavigationViewPageProvider , PageService > ( ) ;
                                                                           services.AddSingleton < INavigationService , NavigationService > ( ) ;

                                                                           // Theme manipulation
                                                                           services.AddSingleton < IThemeService , ThemeService > ( ) ;

                                                                           // TaskBar manipulation
                                                                           services.AddSingleton < ITaskBarService , TaskBarService > ( ) ;

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
                                                                           services.AddSingleton < IToastService , ToastService > ( ) ;
                                                                           services.AddSingleton < SettingsChanges > ( ) ;
                                                                           services.AddSingleton < IObserveSettingsChanges > ( GetSettingsChanged ) ;
                                                                           services.AddSingleton < INotifySettingsChanges > ( GetSettingsChanged ) ;
                                                                           services.AddSingleton < ISettingsManager , SettingsManager > ( ) ;
                                                                           services.AddSingleton < ILoggingSettingsManager , LoggingSettingsManager > ( ) ;
                                                                           services.AddSingleton < ICommonApplicationData , CommonApplicationData > ( ) ;
                                                                           services.AddSingleton < ISettingsStorage , SettingsStorage > ( ) ;
                                                                           services.AddSingleton < ITaskbarIconProvider , TaskbarIconProvider > ( ) ;
                                                                           services.AddSingleton < IUiDeskManager , UiDeskManager > ( ) ;
                                                                           services.AddSingleton < IDynamicIconCreator , DynamicIconCreator > ( ) ;
                                                                           services.AddSingleton ( _ => CreateScheduler ( ) ) ;
                                                                           services.AddSingleton ( CreateTaskbarIconProvider ) ;
                                                                           services.AddTransient < IDeviceNameConverter , DeviceNameConverter > ( ) ;
                                                                           services.AddTransient < IDoubleToUIntConverter , DoubleToUIntConverter > ( ) ;
                                                                           services.AddTransient < IStringToUIntConverter , StringToUIntConverter > ( ) ;
                                                                           services.AddTransient < IDeviceAddressToULongConverter , DeviceAddressToULongConverter > ( ) ;
                                                                           services.AddSingleton ( provider => new Func < IDeskProvider > ( provider.GetRequiredService < IDeskProvider > ) ) ;
                                                                           services.AddSingleton < IFileSystem , FileSystem > ( ) ;
                                                                           services.AddTransient < IThemeSwitcher , ThemeSwitcher > ( ) ;
                                                                           services.AddTransient < ISettingsSynchronizer , SettingsSynchronizer > ( ) ;
                                                                           services.AddSingleton < IApplicationThemeManager , MyApplicationThemeManager > ( ) ;
                                                                           services.AddSingleton < Func < TimerCallback , object ? , TimeSpan , TimeSpan , ITimer > > ( _ =>
                                                                                                           ( callback ,
                                                                                                               state ,
                                                                                                               dueTime ,
                                                                                                               period ) => new Timer ( callback ,
                                                                                                                    state ,
                                                                                                                    dueTime ,
                                                                                                                    period ) ) ;
                                                                       } ).Build ( ) ;

    private readonly ILogger _logger = AppLogger ;

    private static Mutex ? _singleInstanceMutex ;

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
    private async void OnStartup ( object sender , StartupEventArgs args )
    {
        try
        {
            if ( ! EnsureSingleInstance ( ) )
            {
                _logger.Information ( "##### Application already running!" ) ;
                Current.Shutdown ( ) ;
                return ;
            }

            await Host.StartAsync ( ) ;

            UnhandledExceptionsHandler.RegisterGlobalExceptionHandling ( ) ;

            _logger.Information ( "##### Startup..." ) ;

            var notifyIcon = FindNotifyIcon ( ) ; // todo maybe we can get the icon from the view?

            var main = GetService < IdasenDeskWindowViewModel > ( ) ;

            main!.Initialize ( notifyIcon ) ;

            var settings = GetService < SettingsViewModel > ( ) ;

            await settings!.InitializeAsync ( CancellationToken.None ) ;

            var versionProvider = GetVersionProvider ( ) ;

            CurrentWindow.StateChanged += OnStateChanged_HideOnMinimize ;

            _logger.Information ( "##### Idasen.SystemTray {Version}" ,
                                  versionProvider.GetVersion ( ) ) ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                            "Failed to start application" ) ;
        }
    }

    private bool EnsureSingleInstance ( )
    {
        var mutexName = $"Global\\{Constants.ApplicationName}_SingleInstance" ;

        try
        {
            _singleInstanceMutex = new Mutex ( true , mutexName , out var createdNew ) ;

            return createdNew ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                            "Failed to create/open single-instance mutex {MutexName}" ,
                            mutexName ) ;
            // In doubt, allow startup to avoid blocking the app due to mutex issues.
            return true ;
        }
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
        try
        {
            _logger.Information ( "##### Exiting..." ) ;

            await Host.StopAsync ( ) ;

            _logger.Information ( "##### Stopped." ) ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                            "Failed to stop application" ) ;
        }
        finally
        {
            try
            {
                _singleInstanceMutex?.ReleaseMutex ( ) ;
                _singleInstanceMutex?.Dispose ( ) ;
                _singleInstanceMutex = null ;
            }
            catch
            {
                // ignore mutex release errors
            }
        }
    }

    private void OnStateChanged_HideOnMinimize ( object ? sender , EventArgs e )
    {
        if ( CurrentWindow.WindowState != WindowState.Minimized )
            return ;

        CurrentWindow.Visibility  = Visibility.Hidden ;
        CurrentWindow.WindowState = WindowState.Normal ;
    }

    private static NotifyIcon FindNotifyIcon ( )
    {
        if ( ! CurrentWindow.CheckAccess ( ) )
        {
            // Ensure we access visual tree on UI thread and return the result
            return CurrentWindow.Dispatcher.Invoke ( FindNotifyIcon ) ;
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