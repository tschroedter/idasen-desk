using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Windows.Controls ;
using System.Windows.Input ;
using System.Windows.Threading ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Views.Pages ;
using JetBrains.Annotations ;
using Wpf.Ui.Controls ;
using ILogger = Serilog.ILogger ;
using MenuItem = Wpf.Ui.Controls.MenuItem ;
using MessageBox = Wpf.Ui.Controls.MessageBox ;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult ;
using NotifyIcon = Wpf.Ui.Tray.Controls.NotifyIcon ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

[ ExcludeFromCodeCoverage ]
public partial class IdasenDeskWindowViewModel : ObservableObject , IAsyncDisposable
{
    private readonly NavigationViewItem ?    _closeWindowViewItem ;
    private readonly NavigationViewItem ?    _connectViewItem ;
    private readonly NavigationViewItem ?    _disconnectViewItem ;
    private readonly NavigationViewItem ?    _exitViewItem ;
    private readonly ILogger                 _logger ;
    private readonly IScheduler              _scheduler ;
    private readonly IObserveSettingsChanges _settingsChanges ;
    private readonly NavigationViewItem ?    _sitViewItem ;
    private readonly NavigationViewItem ?    _standViewItem ;
    private readonly NavigationViewItem ?    _stopViewItem ;
    private readonly NavigationViewItem ?    _custom1ViewItem ;
    private readonly NavigationViewItem ?    _custom2ViewItem ;
    private readonly IUiDeskManager          _uiDeskManager ;

    private IDisposable ? _advancedSubscription ;

    [ ObservableProperty ]
    private string _applicationTitle = "Idasen Desk" ;

    [ ObservableProperty ]
    private ObservableCollection < object > _footerMenuItems = [] ;

    private bool _isInitialized ;

    private IDisposable ? _lockSubscription ;

    [ ObservableProperty ]
    private ObservableCollection < object > _menuItems = [] ;

    [ UsedImplicitly ]
    private IDisposable ? _onErrorChanged ;

    [ ObservableProperty ]
    private ObservableCollection < MenuItem > _trayMenuItems = [] ;

    public IdasenDeskWindowViewModel ( ILogger                 logger ,
                                       IServiceProvider        serviceProvider ,
                                       IUiDeskManager          uiDeskManager ,
                                       IObserveSettingsChanges settingsChanges ,
                                       IScheduler              scheduler )
    {
        Guard.ArgumentNotNull ( serviceProvider ,
                                nameof ( serviceProvider ) ) ;
        Guard.ArgumentNotNull ( uiDeskManager ,
                                nameof ( uiDeskManager ) ) ;

        _logger          = logger ;
        _uiDeskManager   = uiDeskManager ;
        _settingsChanges = settingsChanges ;
        _scheduler       = scheduler ;

        var homeViewItem = new NavigationViewItem
        {
            Content        = "Home" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.Home20 } ,
            TargetPageType = typeof ( HomePage )
        } ;

        _sitViewItem = new NavigationViewItem
        {
            Content        = "Sit" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleDown20 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to move the desk to the sitting position."
        } ;
        _sitViewItem.MouseDoubleClick += OnClickSitViewItem ;

        _standViewItem = new NavigationViewItem
        {
            Content        = "Stand" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleUp20 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to move the desk to the standing position."
        } ;
        _standViewItem.MouseDoubleClick += OnClickStandViewItem ;

        _custom1ViewItem = new NavigationViewItem
        {
            Content        = "Custom1" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowBidirectionalUpDown24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to move the desk to the Custom1 position."
        } ;
        _custom1ViewItem.MouseDoubleClick += OnClickCustom1ViewItem ;

        _custom2ViewItem = new NavigationViewItem
        {
            Content        = "Custom 2" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowBidirectionalUpDown24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to move the desk to the Custom 2 position."
        } ;
        _custom2ViewItem.MouseDoubleClick += OnClickCustom2ViewItem ;

        _connectViewItem = new NavigationViewItem
        {
            Content        = "Connect" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.PlugConnected24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to connect to desk."
        } ;
        _connectViewItem.MouseDoubleClick += OnClickConnectViewItem ;

        _disconnectViewItem = new NavigationViewItem
        {
            Content        = "Disconnect" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.PlugDisconnected24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to disconnect desk."
        } ;
        _disconnectViewItem.MouseDoubleClick += OnClickDisconnectViewItem ;

        _closeWindowViewItem = new NavigationViewItem
        {
            Content        = "Hide" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.SlideHide24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to close this window."
        } ;
        _closeWindowViewItem.MouseDoubleClick += OnClickCloseViewItem ;

        var settingsViewItem = new NavigationViewItem
        {
            Content        = "Settings" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.Settings24 } ,
            TargetPageType = typeof ( SettingsPage ) ,
            ToolTip        = "Double-Click to see settings."
        } ;

        _stopViewItem = new NavigationViewItem
        {
            Content        = "Stop" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.Stop24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to stop the desk when moving."
        } ;
        _stopViewItem.MouseDoubleClick += OnClickStopViewItem ;

        _exitViewItem = new NavigationViewItem
        {
            Content        = "Exit" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.CallInbound24 } ,
            TargetPageType = typeof ( StatusPage ) ,
            ToolTip        = "Double-Click to exit the application."
        } ;
        _exitViewItem.MouseDoubleClick += OnClickExit ;

        _menuItems.Add ( homeViewItem ) ;
        _menuItems.Add ( settingsViewItem ) ;
        _menuItems.Add ( new CustomSeparatorMenuItem () );
        _menuItems.Add ( _standViewItem ) ;
        _menuItems.Add ( _sitViewItem ) ;
        _menuItems.Add ( _custom1ViewItem ) ;
        _menuItems.Add ( _custom2ViewItem ) ;
        _menuItems.Add ( _stopViewItem ) ;
        _menuItems.Add (new CustomSeparatorMenuItem());
        _menuItems.Add ( _connectViewItem ) ;
        _menuItems.Add ( _disconnectViewItem ) ;
        _menuItems.Add (new CustomSeparatorMenuItem());
        _menuItems.Add ( _closeWindowViewItem ) ;

        _footerMenuItems.Add ( _exitViewItem ) ;

        TrayMenuItems =
        [
            new MenuItem { Header = "Stand" , Command         = StandingCommand } ,
            new MenuItem { Header = "Sit" , Command           = SeatingCommand } ,
            new MenuItem { Header = "Custom 1" , Command      = Custom1Command } ,
            new MenuItem { Header = "Custom 2" , Command      = Custom2Command } ,
            new MenuItem { Header = "Stop" , Command          = StopCommand } ,
            new CustomSeparatorMenuItem (  ),
            new MenuItem { Header = "Connect" , Command       = ConnectCommand } ,
            new MenuItem { Header = "Disconnect" , Command    = DisconnectCommand } ,
            new CustomSeparatorMenuItem (  ),
            new MenuItem { Header = "Show Settings" , Command = ShowSettingsCommand } ,
            new MenuItem { Header = "Hide Settings" , Command = HideSettingsCommand } ,
            new CustomSeparatorMenuItem (  ),
            new MenuItem { Header = "Exit" , Command          = ExitApplicationCommand }
        ] ;
    }

    /// <summary>
    ///     Shows a window, if none is already open.
    /// </summary>
    public ICommand ShowSettingsCommand =>
        new DelegateCommand ( DoShowSettings ,
                              CanShowSettings ) ;

    /// <summary>
    ///     Hides the main window. This command is only enabled if a window is open.
    /// </summary>
    public ICommand HideSettingsCommand =>
        new DelegateCommand ( DoHideSettings ,
                              CanHideSettings ) ;

    /// <summary>
    ///     Connects to the Idasen Desk.
    /// </summary>
    public ICommand ConnectCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async _ => await _uiDeskManager.AutoConnectAsync ( ) ,
                              CanExecuteConnect ) ;

    /// <summary>
    ///     Disconnects from the Idasen Desk.
    /// </summary>
    public ICommand DisconnectCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( DoDisconnect,
                              CanExecuteDisconnect ) ;

    /// <summary>
    ///     Moves the desk to the standing height.
    /// </summary>
    public ICommand StandingCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async _ => await _uiDeskManager.StandAsync ( ) ,
                              CanExecuteStanding ) ;

    /// <summary>
    ///     Moves the desk to the seating height.
    /// </summary>
    public ICommand SeatingCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async _ => await _uiDeskManager.SitAsync ( ) ,
                              CanExecuteSeating ) ;

    /// <summary>
    ///     Moves the desk to the custom 1 height.
    /// </summary>
    public ICommand Custom1Command =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async _ => await _uiDeskManager.Custom1Async ( ) ,
                              CanExecuteStanding ) ;

    /// <summary>
    ///     Moves the desk to the eating height (currently uses SitAsync until specific implementation exists).
    /// </summary>
    public ICommand Custom2Command =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async _ => await _uiDeskManager.SitAsync ( ) ,
                              CanExecuteSeating ) ;

    /// <summary>
    ///     Stop the desk moving.
    /// </summary>
    public ICommand StopCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async _ => await _uiDeskManager.StopAsync ( ) ,
                              CanExecuteStop ) ;

    /// <summary>
    ///     Shuts down the application.
    /// </summary>
    public ICommand ExitApplicationCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( DoExitApplication ,
                              _ => true ) ;

    public async ValueTask DisposeAsync ( )
    {
        _logger.Information ( "Disposing..." ) ;

        if ( _sitViewItem != null )
            _sitViewItem.MouseDoubleClick -= OnClickSitViewItem ;
        if ( _standViewItem != null )
            _standViewItem.MouseDoubleClick -= OnClickStandViewItem ;
        if ( _custom1ViewItem != null )
            _custom1ViewItem.MouseDoubleClick -= OnClickCustom1ViewItem ;
        if ( _custom2ViewItem != null )
            _custom2ViewItem.MouseDoubleClick -= OnClickCustom2ViewItem ;
        if ( _stopViewItem != null )
            _stopViewItem.MouseDoubleClick -= OnClickStopViewItem ;
        if ( _connectViewItem != null )
            _connectViewItem.MouseDoubleClick -= OnClickConnectViewItem ;
        if ( _disconnectViewItem != null )
            _disconnectViewItem.MouseDoubleClick -= OnClickDisconnectViewItem ;
        if ( _closeWindowViewItem != null )
            _closeWindowViewItem.MouseDoubleClick -= OnClickCloseViewItem ;
        if ( _exitViewItem != null )
            _exitViewItem.MouseDoubleClick -= OnClickExit ;

        await CastAndDispose ( _uiDeskManager ) ;
        await CastAndDispose ( _advancedSubscription ) ;
        await CastAndDispose ( _lockSubscription ) ;
        await CastAndDispose ( _onErrorChanged ) ;
    }

    private async static ValueTask CastAndDispose ( IDisposable ? resource )
    {
        if ( resource is IAsyncDisposable resourceAsyncDisposable )
            await resourceAsyncDisposable.DisposeAsync ( ) ;
        else
            resource?.Dispose ( ) ;
    }


    private bool CanExecuteConnect ( object ? _ )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: false } ;
    }

    private bool CanExecuteDisconnect ( object ? _ )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private bool CanExecuteStanding ( object ? _ )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private bool CanExecuteSeating ( object ? _ )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private bool CanExecuteStop ( object ? _ )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private void OnClickExit ( object sender , MouseButtonEventArgs e )
    {
        DoExitApplication ( e ) ;
    }

    private void OnClickCloseViewItem ( object sender , MouseButtonEventArgs e )
    {
        DoHideWindow ( ) ;
    }

    private void DoHideWindow ( )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Hide ?" ,
            Content           = "Do you want to hide this window?" ,
            PrimaryButtonText = "Hide"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.HideAsync ( ) ;
                                                   } ) ;
    }

    private static bool CanShowSettings ( object ? _ )
    {
        return Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility != Visibility.Visible ;
    }

    private static bool CanHideSettings ( object ? _ )
    {
        return Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility != Visibility.Hidden ;
    }

    private void DoShowSettings ( object ? _ )
    {
        if ( Application.Current.MainWindow == null )
            return ;

        _logger.Debug ( $"{nameof ( ShowSettingsCommand )}" ) ;

        Application.Current.MainWindow.Visibility = Visibility.Visible ;
    }

    private void DoHideSettings ( object ? _ )
    {
        if ( Application.Current.MainWindow == null )
            return ;

        _logger.Debug ( $"{nameof ( HideSettingsCommand )}" ) ;

        Application.Current.MainWindow.Visibility = Visibility.Hidden ;
    }

    private void DoDisconnect(object? _)
    {
        if (!_uiDeskManager.IsInitialize)
        {
            return;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Disconnect ?",
            Content           = "Do you want to disconnect from the desk?",
            PrimaryButtonText = "Disconnect"
        };

        Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                                                 {
                                                     var result = await uiMessageBox.ShowDialogAsync();

                                                     if (result != MessageBoxResult.Primary)
                                                     {
                                                         return;
                                                     }

                                                     await _uiDeskManager.DisconnectAsync();
                                                 });
    }

    private void DoExitApplication ( object ? _ )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Exit ?" ,
            Content           = "Do you want to exit the application?" ,
            PrimaryButtonText = "Exit"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.ExitAsync ( ) ;
                                                   } ) ;
    }

    private void OnClickSitViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Sit ?" ,
            Content           = "Do you want to move the desk into the sitting position?" ,
            PrimaryButtonText = "Sit"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.SitAsync ( ) ;
                                                   } ) ;
    }

    private void OnClickStandViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Stand ?" ,
            Content           = "Do you want to move the desk into the sitting position?" ,
            PrimaryButtonText = "Stand"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.StandAsync ( ) ;
                                                   } ) ;
    }

    private void OnClickCustom1ViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Custom1" ,
            Content           = "Do you want to move the desk into the Custom1 position?" ,
            PrimaryButtonText = "Move"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.Custom1Async( ) ;
                                                   } ) ;
    }

    private void OnClickCustom2ViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Custom 2 ?" ,
            Content           = "Do you want to move the desk into the custom 2 position?" ,
            PrimaryButtonText = "Move"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.Custom2Async ( ) ;
                                                   } ) ;
    }

    private void OnClickStopViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) => { await _uiDeskManager.StopAsync ( ) ; } ) ;
    }

    private void OnClickConnectViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Connect ?" ,
            Content           = "Do you want to connect to the desk?" ,
            PrimaryButtonText = "Connect"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.AutoConnectAsync ( ) ;
                                                   } ) ;
    }

    private void OnClickDisconnectViewItem ( object sender , RoutedEventArgs e )
    {
        if ( ! _uiDeskManager.IsInitialize )
        {
            return ;
        }

        var uiMessageBox = new MessageBox
        {
            Title             = "Disconnect ?" ,
            Content           = "Do you want to disconnect from the desk?" ,
            PrimaryButtonText = "Disconnect"
        } ;

        Dispatcher.CurrentDispatcher.InvokeAsync ( async ( ) =>
                                                   {
                                                       var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                       if ( result != MessageBoxResult.Primary )
                                                       {
                                                           return ;
                                                       }

                                                       await _uiDeskManager.DisconnectAsync ( ) ;
                                                   } ) ;
    }

    public IdasenDeskWindowViewModel Initialize ( NotifyIcon notifyIcon )
    {
        if ( _isInitialized )
            return this ;

        _isInitialized = true ;

        _logger.Debug ( $"{nameof ( IdasenDeskWindowViewModel )}: Initializing..." ) ;

        _uiDeskManager.Initialize ( notifyIcon ) ;

        notifyIcon.Menu = new ContextMenu
        {
            ItemsSource = TrayMenuItems
        } ;

        _advancedSubscription = _settingsChanges.AdvancedSettingsChanged
                                                .ObserveOn ( _scheduler )
                                                .Subscribe ( async hasChanged => await OnAdvancedSettingsChanged ( hasChanged ) ) ;

        _lockSubscription = _settingsChanges.LockSettingsChanged
                                            .ObserveOn ( _scheduler )
                                            .Subscribe ( async isLocked => await OnLockSettingsChanged ( isLocked ) ) ;

        return this ;
    }

    private async Task OnAdvancedSettingsChanged ( bool hasChanged )
    {
        try
        {
            _logger.Debug ( $"{nameof ( OnAdvancedSettingsChanged )}: hasChanged={hasChanged}" ) ;

            // ReSharper disable once MethodSupportsCancellation
            await Task.Delay ( 3000 )
                      .ConfigureAwait ( false ) ;

            await _uiDeskManager.DisconnectAsync ( ) ;

            await _uiDeskManager.AutoConnectAsync ( ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed  to reconnect after advanced settings change." ) ;
        }
    }

    private async Task OnLockSettingsChanged ( bool isLocked )
    {
        try
        {
            _logger.Debug ( $"{nameof ( OnLockSettingsChanged )}: hasChanged={isLocked}" ) ;

            if ( isLocked )
                await _uiDeskManager.MoveLockAsync ( ) ;
            else
                await _uiDeskManager.MoveUnlockAsync ( ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed  to lock/unlock after locked settings change." ) ;
        }
    }
}