using System.Collections.ObjectModel ;
using System.Windows.Input ;
using Autofac ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Settings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.SystemTray.Win11.Views.Pages ;
using JetBrains.Annotations ;
using Wpf.Ui.Controls ;
using ILogger = Serilog.ILogger ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    public MainWindowViewModel ( ISettingsManager     manager ,
                                 IUiDeskManager uiDeskManager)
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;
        Guard.ArgumentNotNull ( uiDeskManager ,
                                nameof ( uiDeskManager ) ) ;

        _manager       = manager ;
        _uiDeskManager = uiDeskManager ;
    }

    [ ObservableProperty ]
    private string _applicationTitle = "Idasen Desk" ;

    [ ObservableProperty ]
    private ObservableCollection < object > _footerMenuItems =
    [
        new NavigationViewItem
        {
            Content        = "Settings" ,
            Icon           = new SymbolIcon { Symbol = SymbolRegular.Settings24 } ,
            TargetPageType = typeof ( SettingsPage )
        }
    ] ;

    private readonly IUiDeskManager       _uiDeskManager ;
    private readonly ISettingsManager?    _manager;
    private          ILogger ?            _logger ;

    [ ObservableProperty ]
    private ObservableCollection < object > _menuItems =
    [
        new NavigationViewItem
        {
            Content = "Home" ,
            Icon = new SymbolIcon
            {
                Symbol = SymbolRegular.Home24
            } ,
            TargetPageType = typeof ( DashboardPage )
        } ,
        new NavigationViewItem
        {
            Content = "Connect" ,
            Icon = new SymbolIcon
            {
                Symbol = SymbolRegular.PlugConnected24
            } ,
            TargetPageType = typeof ( ConnectPage )
        } ,
        new NavigationViewItem
        {
            Content = "Disconnect" ,
            Icon = new SymbolIcon
            {
                Symbol = SymbolRegular.PlugDisconnected24
            } ,
            TargetPageType = typeof ( DisconnectPage )
        } ,
        new NavigationViewItem
        {
            Content = "Stand" ,
            Icon = new SymbolIcon
            {
                Symbol = SymbolRegular.ArrowCircleUp20
            } ,
            TargetPageType = typeof ( StandPage )
        } ,
        new NavigationViewItem
        {
            Content = "Sit" ,
            Icon = new SymbolIcon
            {
                Symbol = SymbolRegular.ArrowCircleDown20
            } ,
            TargetPageType = typeof ( SitPage )
        }
    ] ;

    private TaskbarIcon ? _taskbarIcon ;

    [ UsedImplicitly ]
    private IDisposable ? _onErrorChanged ;

    [ ObservableProperty ]
    private ObservableCollection < MenuItem > _trayMenuItems = // todo do we need this or does this replace the current notifyicon?
        [new() { Header = "Home" , Tag = "tray_home" }] ;

    /// <summary>
    ///     Moves the desk to the standing height.
    /// </summary>
    public ICommand StandingCommand
    {
        get
        {
            return new DelegateCommand
            {
                // ReSharper disable once AsyncVoidLambda
                CommandAction  = async ( ) => await _uiDeskManager.Standing ( ) ,
                CanExecuteFunc = ( ) => _uiDeskManager.IsInitialize
            } ;
        }
    }

    /// <summary>
    ///     Moves the desk to the seating height.
    /// </summary>
    public ICommand SeatingCommand
    {
        get
        {
            return new DelegateCommand
            {
                // ReSharper disable once AsyncVoidLambda
                CommandAction  = async ( ) => await _uiDeskManager.Seating( ) ,
                CanExecuteFunc = ( ) => _uiDeskManager.IsInitialize
            } ;
        }
    }

    public bool IsInitialize => _logger != null && _manager != null ; // todo  && _provider != null ;

    public MainWindowViewModel Initialize ( IContainer container , TaskbarIcon taskbarIcon)
    {
        Guard.ArgumentNotNull ( container ,
                                nameof ( container ) ) ;
        Guard.ArgumentNotNull(taskbarIcon,
                              nameof(taskbarIcon));

        _taskbarIcon = taskbarIcon;

        _logger          = container.Resolve<ILogger>();
        _logger?.Debug ( $"{nameof(MainWindowViewModel)}: Initializing..." ) ;

        _uiDeskManager.Initialize ( container, taskbarIcon ) ;

        return this ;
    }

    public void Dispose ( )
    {
        _logger?.Information ( "Disposing..." ) ;

        _uiDeskManager.Disconnect (  );
        _uiDeskManager.Dispose ( ) ;

        _taskbarIcon?.Dispose ( ) ;
    }
}

