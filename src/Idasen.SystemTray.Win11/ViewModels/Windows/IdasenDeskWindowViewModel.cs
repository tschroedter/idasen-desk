using System.Collections.ObjectModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Windows.Controls ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Views.Pages ;
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
    private readonly NavigationViewItem ? _closeWindowViewItem ;
    private readonly NavigationViewItem ? _connectViewItem ;
    private readonly NavigationViewItem ? _custom1ViewItem ;
    private readonly NavigationViewItem ? _custom2ViewItem ;
    private readonly NavigationViewItem ? _disconnectViewItem ;
    private readonly NavigationViewItem ? _exitViewItem ;

    private readonly ILogger _logger ;

    private readonly MenuItem                _menuItemConnect ;
    private readonly MenuItem                _menuItemCustom1 ;
    private readonly MenuItem                _menuItemCustom2 ;
    private readonly MenuItem                _menuItemDisconnect ;
    private readonly MenuItem                _menuItemHide ;
    private readonly MenuItem                _menuItemShow ;
    private readonly MenuItem                _menuItemSit ;
    private readonly MenuItem                _menuItemStand ;
    private readonly MenuItem                _menuItemStop ;
    private readonly IScheduler              _scheduler ;
    private readonly IObserveSettingsChanges _settingsChanges ;
    private readonly ISettingsManager        _settingsManager ;

    // Cached commands
    private readonly NavigationViewItem ? _sitViewItem ;
    private readonly NavigationViewItem ? _standViewItem ;
    private readonly NavigationViewItem ? _stopViewItem ;
    private readonly IUiDeskManager       _uiDeskManager ;

    private IDisposable ? _advancedSubscription ;

    [ ObservableProperty ] private string _applicationTitle = "Idasen Desk" ;

    private ContextMenu ? _contextMenu ;

    [ ObservableProperty ] private ObservableCollection < object > _footerMenuItems = [] ;

    private bool _isActionInProgress ;

    private bool          _isInitialized ;
    private IDisposable ? _lockSubscription ;

    [ ObservableProperty ] private ObservableCollection < object > _menuItems = [] ;

    private NotifyIcon ? _notifyIcon ;

    [ ObservableProperty ] private ObservableCollection < MenuItem > _trayMenuItems = [] ;

    public IdasenDeskWindowViewModel ( ILogger                 logger ,
                                       IUiDeskManager          uiDeskManager ,
                                       IObserveSettingsChanges settingsChanges ,
                                       IScheduler              scheduler ,
                                       ISettingsManager        settingsManager )
    {
        _logger          = logger ;
        _uiDeskManager   = uiDeskManager ;
        _settingsChanges = settingsChanges ;
        _scheduler       = scheduler ;
        _settingsManager = settingsManager ;

        var homeViewItem = new NavigationViewItem
        {
            Content        = "Home" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.Home20 } ,
            TargetPageType = typeof ( HomePage )
        } ;

        _sitViewItem = new NavigationViewItem
        {
            Content        = "Sit" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleDown24 } ,
            TargetPageType = typeof ( SettingsPage ) ,
            ToolTip        = "Double-Click to move the desk to the sitting position."
        } ;
        _sitViewItem.MouseDoubleClick += OnClickSitViewItem ;

        _standViewItem = new NavigationViewItem
        {
            Content        = "Stand" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleUp24 } ,
            TargetPageType = typeof ( SettingsPage ) ,
            ToolTip        = "Double-Click to move the desk to the standing position."
        } ;
        _standViewItem.MouseDoubleClick += OnClickStandViewItem ;

        _custom1ViewItem = new NavigationViewItem
        {
            Content        = "Custom 1" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleLeft24 } ,
            TargetPageType = typeof ( SettingsPage ) ,
            ToolTip        = "Double-Click to move the desk to the Custom1 position."
        } ;
        _custom1ViewItem.MouseDoubleClick += OnClickCustom1ViewItem ;

        _custom2ViewItem = new NavigationViewItem
        {
            Content        = "Custom 2" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleRight24 } ,
            TargetPageType = typeof ( SettingsPage ) ,
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
        _menuItems.Add ( new CustomSeparatorMenuItem ( ) ) ;
        _menuItems.Add ( _standViewItem ) ;
        _menuItems.Add ( _sitViewItem ) ;
        _menuItems.Add ( _custom1ViewItem ) ;
        _menuItems.Add ( _custom2ViewItem ) ;
        _menuItems.Add ( _stopViewItem ) ;
        _menuItems.Add ( new CustomSeparatorMenuItem ( ) ) ;
        _menuItems.Add ( _connectViewItem ) ;
        _menuItems.Add ( _disconnectViewItem ) ;
        _menuItems.Add ( new CustomSeparatorMenuItem ( ) ) ;
        _menuItems.Add ( _closeWindowViewItem ) ;

        _footerMenuItems.Add ( _exitViewItem ) ;

        // Cache commands
        // ReSharper disable AsyncVoidLambda
        ShowSettingsCommand = new DelegateCommand ( DoShowSettings ,
                                                    CanShowSettings ) ;
        HideSettingsCommand = new DelegateCommand ( DoHideSettings ,
                                                    CanHideSettings ) ;
        ICommand connectCommand = new DelegateCommand ( async _ => await _uiDeskManager.AutoConnectAsync ( ) ,
                                                        CanExecuteConnect ) ;
        ICommand disconnectCommand = new DelegateCommand ( DoDisconnect ,
                                                           CanExecuteDisconnect ) ;
        ICommand standingCommand = new DelegateCommand ( async _ => await _uiDeskManager.StandAsync ( ) ,
                                                         CanExecuteStanding ) ;
        ICommand seatingCommand = new DelegateCommand ( async _ => await _uiDeskManager.SitAsync ( ) ,
                                                        CanExecuteSeating ) ;
        ICommand custom1Command = new DelegateCommand ( async _ => await _uiDeskManager.Custom1Async ( ) ,
                                                        CanExecuteStanding ) ;
        ICommand custom2Command = new DelegateCommand ( async _ => await _uiDeskManager.Custom2Async ( ) ,
                                                        CanExecuteStanding ) ;
        ICommand stopCommand = new DelegateCommand ( async _ => await _uiDeskManager.StopAsync ( ) ,
                                                     CanExecuteStop ) ;
        ICommand exitApplicationCommand = new DelegateCommand ( DoExitApplication ,
                                                                _ => true ) ;

        // ReSharper restore AsyncVoidLambda
        _menuItemConnect = new MenuItem
        {
            Header = "Connect" , Command = connectCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.PlugConnected24 }
        } ;
        _menuItemDisconnect = new MenuItem
        {
            Header = "Disconnect" , Command = disconnectCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.PlugDisconnected24 }
        } ;
        _menuItemShow = new MenuItem
        {
            Header = "Show Settings" , Command = ShowSettingsCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.SlideTransition24 }
        } ;
        _menuItemHide = new MenuItem
        {
            Header = "Hide Settings" , Command = HideSettingsCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.SlideHide24 }
        } ;

        _menuItemStand = new MenuItem
        {
            Header = "Stand" , Command = standingCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleUp24 }
        } ;
        _menuItemSit = new MenuItem
        {
            Header = "Sit" , Command = seatingCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleDown24 }
        } ;
        _menuItemCustom1 = new MenuItem
        {
            Header = "Custom 1" , Command = custom1Command ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleLeft24 }
        } ;
        _menuItemCustom2 = new MenuItem
        {
            Header = "Custom 2" , Command = custom2Command ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.ArrowCircleRight24 }
        } ;
        _menuItemStop = new MenuItem
            { Header = "Stop" , Command = stopCommand , Icon = new SymbolIcon { Symbol = SymbolRegular.Stop24 } } ;
        var menuItemExit = new MenuItem
        {
            Header = "Exit" , Command = exitApplicationCommand ,
            Icon   = new SymbolIcon { Symbol = SymbolRegular.CallInbound24 }
        } ;

        TrayMenuItems =
        [
            _menuItemStand ,
            _menuItemSit ,
            _menuItemCustom1 ,
            _menuItemCustom2 ,
            _menuItemStop ,
            new CustomSeparatorMenuItem ( ) ,
            _menuItemConnect ,
            _menuItemDisconnect ,
            new CustomSeparatorMenuItem ( ) ,
            _menuItemShow ,
            _menuItemHide ,
            new CustomSeparatorMenuItem ( ) ,
            menuItemExit
        ] ;
    }

    /// <summary>
    ///     Shows a window, if none is already open.
    /// </summary>
    public ICommand ShowSettingsCommand { get ; }

    /// <summary>
    ///     Hides the main window. This command is only enabled if a window is open.
    /// </summary>
    public ICommand HideSettingsCommand { get ; }

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

        if ( _contextMenu != null )
        {
            _contextMenu.Opened -= OnContextMenuOpened ;

            // If we set a context menu on the tray icon, clear it to avoid retaining the instance
            if ( _notifyIcon != null &&
                 ReferenceEquals ( _notifyIcon.Menu ,
                                   _contextMenu ) )
                _notifyIcon.Menu = null ;

            _contextMenu = null ;
        }

        _notifyIcon = null ;

        await CastAndDispose ( _uiDeskManager ) ;
        await CastAndDispose ( _advancedSubscription ) ;
        await CastAndDispose ( _lockSubscription ) ;

        GC.SuppressFinalize ( this ) ;
    }

    private static async ValueTask CastAndDispose ( IDisposable ? resource )
    {
        if ( resource is IAsyncDisposable resourceAsyncDisposable )
            await resourceAsyncDisposable.DisposeAsync ( ) ;
        else
            resource?.Dispose ( ) ;
    }


    // Centralize repeated connection checks for CanExecute predicates
    private bool IsReady ( bool mustBeConnected )
    {
        return ! _isActionInProgress                                                 &&
               _uiDeskManager is { IsInitialize: true , IsConnected: var connected } &&
               connected == mustBeConnected ;
    }

    // Centralized busy-guard execution on UI thread
    private void WithBusyGuard ( Func < Task > action )
    {
        if ( _isActionInProgress || ! _uiDeskManager.IsInitialize ) return ;

        Application.Current?.Dispatcher.InvokeAsync ( async ( ) =>
                                                      {
                                                          try
                                                          {
                                                              _isActionInProgress = true ;
                                                              CommandManager.InvalidateRequerySuggested ( ) ;
                                                              await action ( ) ;
                                                          }
                                                          catch ( Exception ex )
                                                          {
                                                              _logger.Error ( ex ,
                                                                              "Action failed" ) ;
                                                          }
                                                          finally
                                                          {
                                                              _isActionInProgress = false ;
                                                              CommandManager.InvalidateRequerySuggested ( ) ;
                                                          }
                                                      } ) ;
    }

    private bool CanExecuteConnect ( object ? _ )
    {
        return IsReady ( false ) ;
    }

    private bool CanExecuteDisconnect ( object ? _ )
    {
        return IsReady ( true ) ;
    }

    private bool CanExecuteStanding ( object ? _ )
    {
        return IsReady ( true ) ;
    }

    private bool CanExecuteSeating ( object ? _ )
    {
        return IsReady ( true ) ;
    }

    private bool CanExecuteStop ( object ? _ )
    {
        return IsReady ( true ) ;
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
        ConfirmAndExecute ( "Hide ?" ,
                            "Do you want to hide this window?" ,
                            "Hide" ,
                            ( ) => _uiDeskManager.HideAsync ( ) ) ;
    }

    private static bool CanShowSettings ( object ? _ )
    {
        return Application.Current.MainWindow            != null &&
               Application.Current.MainWindow.Visibility != Visibility.Visible ;
    }

    private static bool CanHideSettings ( object ? _ )
    {
        return Application.Current.MainWindow            != null &&
               Application.Current.MainWindow.Visibility != Visibility.Hidden ;
    }

    private void DoShowSettings ( object ? _ )
    {
        if ( Application.Current.MainWindow == null )
            return ;

        _logger.Debug ( "Executing command {Command}" ,
                        nameof ( ShowSettingsCommand ) ) ;

        Application.Current.MainWindow.Visibility = Visibility.Visible ;
    }

    private void DoHideSettings ( object ? _ )
    {
        if ( Application.Current.MainWindow == null )
            return ;

        _logger.Debug ( "Executing command {Command}" ,
                        nameof ( HideSettingsCommand ) ) ;

        Application.Current.MainWindow.Visibility = Visibility.Hidden ;
    }

    private void DoDisconnect ( object ? _ )
    {
        ConfirmAndExecute ( "Disconnect ?" ,
                            "Do you want to disconnect from the desk?" ,
                            "Disconnect" ,
                            ( ) => _uiDeskManager.DisconnectAsync ( ) ) ;
    }

    private void DoExitApplication ( object ? _ )
    {
        ConfirmAndExecute ( "Exit ?" ,
                            "Do you want to exit the application?" ,
                            "Exit" ,
                            ( ) => _uiDeskManager.ExitAsync ( ) ) ;
    }

    // Generic helper to show a confirmation and run an async action if confirmed
    private void ConfirmAndExecute ( string title , string content , string primaryButtonText , Func < Task > action )
    {
        if ( _isActionInProgress || ! _uiDeskManager.IsInitialize ) return ;

        var uiMessageBox = new MessageBox
        {
            Title             = title ,
            Content           = content ,
            PrimaryButtonText = primaryButtonText
        } ;

        Application.Current?.Dispatcher.InvokeAsync ( async ( ) =>
                                                      {
                                                          var result = await uiMessageBox.ShowDialogAsync ( ) ;

                                                          if ( result != MessageBoxResult.Primary ) return ;

                                                          WithBusyGuard ( action ) ;
                                                      } ) ;
    }

    // Execute an action without confirmation but with busy guard
    private void ExecuteAsync ( Func < Task > action )
    {
        WithBusyGuard ( action ) ;
    }

    private void OnClickSitViewItem ( object sender , RoutedEventArgs e )
    {
        ConfirmAndExecute ( "Sit ?" ,
                            "Do you want to move the desk into the sitting position?" ,
                            "Sit" ,
                            ( ) => _uiDeskManager.SitAsync ( ) ) ;
    }

    private void OnClickStandViewItem ( object sender , RoutedEventArgs e )
    {
        ConfirmAndExecute ( "Stand ?" ,
                            "Do you want to move the desk into the standing position?" ,
                            "Stand" ,
                            ( ) => _uiDeskManager.StandAsync ( ) ) ;
    }

    private void OnClickCustom1ViewItem ( object sender , RoutedEventArgs e )
    {
        ConfirmAndExecute ( "Custom1" ,
                            "Do you want to move the desk into the Custom1 position?" ,
                            "Move" ,
                            ( ) => _uiDeskManager.Custom1Async ( ) ) ;
    }

    private void OnClickCustom2ViewItem ( object sender , RoutedEventArgs e )
    {
        ConfirmAndExecute ( "Custom 2 ?" ,
                            "Do you want to move the desk into the custom 2 position?" ,
                            "Move" ,
                            ( ) => _uiDeskManager.Custom2Async ( ) ) ;
    }

    private void OnClickStopViewItem ( object sender , RoutedEventArgs e )
    {
        ExecuteAsync ( ( ) => _uiDeskManager.StopAsync ( ) ) ;
    }

    private void OnClickConnectViewItem ( object sender , RoutedEventArgs e )
    {
        ConfirmAndExecute ( "Connect ?" ,
                            "Do you want to connect to the desk?" ,
                            "Connect" ,
                            ( ) => _uiDeskManager.AutoConnectAsync ( ) ) ;
    }

    private void OnClickDisconnectViewItem ( object sender , RoutedEventArgs e )
    {
        ConfirmAndExecute ( "Disconnect ?" ,
                            "Do you want to disconnect from the desk?" ,
                            "Disconnect" ,
                            ( ) => _uiDeskManager.DisconnectAsync ( ) ) ;
    }

    public IdasenDeskWindowViewModel Initialize ( NotifyIcon notifyIcon )
    {
        if ( _isInitialized )
            return this ;

        _isInitialized = true ;

        _logger.Debug ( "{Component} initializing..." ,
                        nameof ( IdasenDeskWindowViewModel ) ) ;

        _uiDeskManager.Initialize ( notifyIcon ) ;

        _notifyIcon = notifyIcon ;

        _contextMenu = new ContextMenu
        {
            ItemsSource = TrayMenuItems
        } ;

        // Attach event handler for ContextMenu.Opened
        _contextMenu.Opened += OnContextMenuOpened ;

        notifyIcon.Menu = _contextMenu ;

        // ReSharper disable AsyncVoidLambda
        _advancedSubscription = _settingsChanges.AdvancedSettingsChanged
                                                .ObserveOn ( _scheduler )
                                                .Subscribe ( async hasChanged =>
                                                                 await OnAdvancedSettingsChanged ( hasChanged ) ) ;

        _lockSubscription = _settingsChanges.LockSettingsChanged
                                            .ObserveOn ( _scheduler )
                                            .Subscribe ( async isLocked => await OnLockSettingsChanged ( isLocked ) ) ;
        // ReSharper restore AsyncVoidLambda

        return this ;
    }

    private void OnContextMenuOpened ( object sender , EventArgs e )
    {
        _logger.Debug ( "Context menu is about to be opened." ) ;

        if ( _uiDeskManager.IsConnected )
        {
            _menuItemConnect.Visibility    = Visibility.Collapsed ;
            _menuItemDisconnect.Visibility = Visibility.Visible ;
        }
        else
        {
            _menuItemConnect.Visibility    = Visibility.Visible ;
            _menuItemDisconnect.Visibility = Visibility.Collapsed ;
        }

        if ( CanShowSettings ( null ) )
        {
            _menuItemShow.Visibility = Visibility.Visible ;
            _menuItemHide.Visibility = Visibility.Collapsed ;
        }
        else
        {
            _menuItemShow.Visibility = Visibility.Collapsed ;
            _menuItemHide.Visibility = Visibility.Visible ;
        }

        var heightSettings = _settingsManager.CurrentSettings.HeightSettings ;

        _menuItemStand.Visibility = heightSettings.StandingIsVisibleInContextMenu
                                        ? Visibility.Visible
                                        : Visibility.Collapsed ;
        _menuItemSit.Visibility = heightSettings.SeatingIsVisibleInContextMenu
                                      ? Visibility.Visible
                                      : Visibility.Collapsed ;
        _menuItemCustom1.Visibility = heightSettings.Custom1IsVisibleInContextMenu
                                          ? Visibility.Visible
                                          : Visibility.Collapsed ;
        _menuItemCustom2.Visibility = heightSettings.Custom2IsVisibleInContextMenu
                                          ? Visibility.Visible
                                          : Visibility.Collapsed ;

        _menuItemStop.Visibility = _settingsManager.CurrentSettings.DeviceSettings.StopIsVisibleInContextMenu
                                       ? Visibility.Visible
                                       : Visibility.Collapsed ;

        var itemsSnapshot = TrayMenuItems.Select ( mi => new
                                          {
                                              Header     = mi.Header?.ToString ( ) ,
                                              Visibility = mi.Visibility.ToString ( )
                                          } )
                                         .ToArray ( ) ;

        _logger.Debug ( "Tray menu snapshot: {TrayItems}" ,
                        itemsSnapshot ) ;

        CommandManager.InvalidateRequerySuggested ( ) ;
    }

    private async Task OnAdvancedSettingsChanged ( bool hasChanged )
    {
        try
        {
            _logger.Debug ( "{Method} hasChanged={HasChanged}" ,
                            nameof ( OnAdvancedSettingsChanged ) ,
                            hasChanged ) ;

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
            _logger.Debug ( "{Method} isLocked={IsLocked}" ,
                            nameof ( OnLockSettingsChanged ) ,
                            isLocked ) ;

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