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

[ExcludeFromCodeCoverage]
public partial class IdasenDeskWindowViewModel : ObservableObject , IDisposable
{
    private static readonly NavigationViewItem HomeViewItem = new ( )
    {
        Content = "Home" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.Home20
        } ,
        TargetPageType = typeof ( HomePage )
    } ;

    private static readonly NavigationViewItem SitViewItem = new ( )
    {
        Content = "Sit" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.ArrowCircleDown20
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to move the desk to the sitting position."
    } ;

    private static readonly NavigationViewItem StandViewItem = new ( )
    {
        Content = "Stand" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.ArrowCircleUp20
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to move the desk to the standing position."
    } ;

    private static readonly NavigationViewItem ConnectViewItem = new ( )
    {
        Content = "Connect" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.PlugConnected24
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to connect to desk."
    } ;

    private static readonly NavigationViewItem DisconnectViewItem = new ( )
    {
        Content = "Disconnect" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.PlugDisconnected24
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to disconnect desk."
    } ;

    private static readonly NavigationViewItem CloseWindowViewItem = new ( )
    {
        Content = "Hide" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.SlideHide24
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to close this window."
    } ;

    private static readonly NavigationViewItem SettingsViewItem = new ( )
    {
        Content = "Settings" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.Settings24
        } ,
        TargetPageType = typeof ( SettingsPage ) ,
        ToolTip        = "Double-Click to see settings."
    } ;

    private static readonly NavigationViewItem StopViewItem = new ( )
    {
        Content = "Stop" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.Stop24
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to stop the desk when moving."
    } ;

    private static readonly NavigationViewItem ExitViewItem = new ( )
    {
        Content = "Exit" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.CallInbound24
        } ,
        TargetPageType = typeof ( StatusPage ) ,
        ToolTip        = "Double-Click to exit the application."
    } ;

    private readonly ILogger _logger ;

    private readonly IObserveSettingsChanges _settingsChanges ;

    private readonly IUiDeskManager _uiDeskManager ;

    private IDisposable ? _advancedSubscription ;

    [ ObservableProperty ]
    private string _applicationTitle = "Idasen Desk" ;

    [ ObservableProperty ]
    private ObservableCollection < object > _footerMenuItems =
    [
        ExitViewItem
    ] ;

    private IDisposable ? _lockSubscription ;

    [ ObservableProperty ]
    private ObservableCollection < object > _menuItems =
    [
        HomeViewItem ,
        SettingsViewItem ,
        ConnectViewItem ,
        DisconnectViewItem ,
        StandViewItem ,
        SitViewItem ,
        StopViewItem ,
        CloseWindowViewItem
    ] ;

    [ UsedImplicitly ]
    private IDisposable ? _onErrorChanged ;

    [ ObservableProperty ]
    private ObservableCollection < MenuItem > _trayMenuItems ;

    public IdasenDeskWindowViewModel ( ILogger                 logger ,
                                       IServiceProvider        serviceProvider ,
                                       IUiDeskManager          uiDeskManager ,
                                       IObserveSettingsChanges settingsChanges )
    {
        Guard.ArgumentNotNull ( serviceProvider ,
                                nameof ( serviceProvider ) ) ;
        Guard.ArgumentNotNull ( uiDeskManager ,
                                nameof ( uiDeskManager ) ) ;

        _logger          = logger ;
        _uiDeskManager   = uiDeskManager ;
        _settingsChanges = settingsChanges ;

        SitViewItem.MouseDoubleClick         += OnClickSitViewItem ;
        StandViewItem.MouseDoubleClick       += OnClickStandViewItem ;
        StopViewItem.MouseDoubleClick        += OnClickStopViewItem ;
        ConnectViewItem.MouseDoubleClick     += OnClickConnectViewItem ;
        DisconnectViewItem.MouseDoubleClick  += OnClickDisconnectViewItem ;
        CloseWindowViewItem.MouseDoubleClick += OnClickCloseViewItem ;
        ExitViewItem.MouseDoubleClick        += OnClickExit ;

        TrayMenuItems =
        [
            new MenuItem { Header = "Show Settings" , Command = ShowSettingsCommand } ,
            new MenuItem { Header = "Hide Settings" , Command = HideSettingsCommand } ,
            new MenuItem { Header = "Connect" , Command       = ConnectCommand } ,
            new MenuItem { Header = "Disconnect" , Command    = DisconnectCommand } ,
            new MenuItem { Header = "Stand" , Command         = StandingCommand } ,
            new MenuItem { Header = "Sit" , Command           = SeatingCommand } ,
            new MenuItem { Header = "Stop" , Command          = StopCommand } ,
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
        new DelegateCommand ( async ( ) => await _uiDeskManager.AutoConnectAsync ( ) ,
                              CanExecuteConnect ) ;

    /// <summary>
    ///     Disconnects from the Idasen Desk.
    /// </summary>
    public ICommand DisconnectCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async ( ) => await _uiDeskManager.DisconnectAsync ( ) ,
                              CanExecuteDisconnect ) ;

    /// <summary>
    ///     Moves the desk to the standing height.
    /// </summary>
    public ICommand StandingCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async ( ) => await _uiDeskManager.StandAsync ( ) ,
                              CanExecuteStanding ) ;

    /// <summary>
    ///     Moves the desk to the seating height.
    /// </summary>
    public ICommand SeatingCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async ( ) => await _uiDeskManager.SitAsync ( ) ,
                              CanExecuteSeating ) ;

    /// <summary>
    ///     Stop the desk moving.
    /// </summary>
    public ICommand StopCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( async ( ) => await _uiDeskManager.StopAsync ( ) ,
                              CanExecuteStop ) ;

    /// <summary>
    ///     Shuts down the application.
    /// </summary>
    public ICommand ExitApplicationCommand =>
        // ReSharper disable once AsyncVoidLambda
        new DelegateCommand ( DoExitApplication ,
                              ( ) => true ) ;

    public void Dispose ( )
    {
        _logger.Information ( "Disposing..." ) ;

        _advancedSubscription?.Dispose ( ) ;
        _lockSubscription?.Dispose ( ) ;

        _uiDeskManager.DisconnectAsync ( ) ;
        _uiDeskManager.Dispose ( ) ;
    }

    private bool CanExecuteConnect ( )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: false } ;
    }

    private bool CanExecuteDisconnect ( )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private bool CanExecuteStanding ( )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private bool CanExecuteSeating ( )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private bool CanExecuteStop ( )
    {
        return _uiDeskManager is { IsInitialize: true , IsConnected: true } ;
    }

    private void OnClickExit ( object sender , MouseButtonEventArgs e )
    {
        DoExitApplication ( ) ;
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

    private static bool CanShowSettings ( )
    {
        return Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility != Visibility.Visible ;
    }

    private static bool CanHideSettings ( )
    {
        return Application.Current.MainWindow != null && Application.Current.MainWindow.Visibility != Visibility.Hidden ;
    }

    private void DoShowSettings ( )
    {
        if ( Application.Current.MainWindow == null )
            return ;

        _logger.Debug ( $"{nameof ( ShowSettingsCommand )}" ) ;

        Application.Current.MainWindow.Visibility = Visibility.Visible ;

        /* todo: implement
        SettingsWindow.Show();
        SettingsWindow.AdvancedSettingsChanged += OnAdvancedSettingsChanged;
        SettingsWindow.LockSettingsChanged     += OnLockSettingsChanged;
        */
    }

    private void DoHideSettings ( )
    {
        if ( Application.Current.MainWindow == null )
            return ;

        _logger.Debug ( $"{nameof ( HideSettingsCommand )}" ) ;

        Application.Current.MainWindow.Visibility = Visibility.Hidden ;

        /* todo: implement
        SettingsWindow.AdvancedSettingsChanged -= OnAdvancedSettingsChanged;
        SettingsWindow.LockSettingsChanged     -= OnLockSettingsChanged;
        SettingsWindow.Hide();
        SettingsWindow = null;
        */
    }

    private void DoExitApplication ( )
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
        _logger.Debug ( $"{nameof ( IdasenDeskWindowViewModel )}: Initializing..." ) ;

        _uiDeskManager.Initialize ( notifyIcon ) ;

        notifyIcon.Menu = new ContextMenu
        {
            ItemsSource = TrayMenuItems
        } ;

        // todo IDisposable right
        _advancedSubscription = _settingsChanges.AdvancedSettingsChanged
                                                .ObserveOn ( Scheduler.Default )
                                                .Subscribe ( OnAdvancedSettingsChanged ) ;
        _lockSubscription = _settingsChanges.LockSettingsChanged
                                            .ObserveOn ( Scheduler.Default )
                                            .Subscribe ( OnLockSettingsChanged ) ;

        return this ;
    }

    private async void OnAdvancedSettingsChanged ( bool hasChanged )
    {
        try
        {
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

    private async void OnLockSettingsChanged ( bool isLocked )
    {
        try
        {
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