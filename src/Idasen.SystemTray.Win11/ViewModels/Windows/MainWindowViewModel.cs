using System.Collections.ObjectModel ;
using System.Windows.Threading ;
using Autofac ;
using Hardcodet.Wpf.TaskbarNotification ;
using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Settings ;
using Idasen.SystemTray.Win11.Views.Pages ;
using JetBrains.Annotations ;
using Wpf.Ui.Controls ;
using ILogger = Serilog.ILogger ;
using MessageBox = Wpf.Ui.Controls.MessageBox ;
using MessageBoxResult = Wpf.Ui.Controls.MessageBoxResult ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public partial class MainWindowViewModel : ObservableObject , IDisposable
{
    private static readonly NavigationViewItem HomeViewItem = new( )
    {
        Content = "Home" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.Home20
        } ,
        TargetPageType = typeof ( HomePage )
    } ;

    private static readonly NavigationViewItem SitViewItem = new( )
    {
        Content = "Sit" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.ArrowCircleDown20
        } ,
        TargetPageType = typeof ( StatusPage )
    } ;

    private static readonly NavigationViewItem StandViewItem = new( )
    {
        Content = "Stand" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.ArrowCircleUp20
        } ,
        TargetPageType = typeof ( StatusPage )
    } ;

    private static readonly NavigationViewItem ConnectViewItem = new( )
    {
        Content = "Connect" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.PlugConnected24
        } ,
        TargetPageType = typeof ( StatusPage )
    } ;

    private static readonly NavigationViewItem DisconnectViewItem = new( )
    {
        Content = "Disconnect" ,
        Icon = new SymbolIcon
        {
            Symbol = SymbolRegular.PlugDisconnected24
        } ,
        TargetPageType = typeof ( StatusPage )
    } ;

    private readonly IUiDeskManager _uiDeskManager ;

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

    private ILogger ? _logger ;

    [ ObservableProperty ]
    private ObservableCollection < object > _menuItems =
    [
        HomeViewItem ,
        ConnectViewItem ,
        DisconnectViewItem ,
        StandViewItem ,
        SitViewItem
    ] ;

    [ UsedImplicitly ]
    private IDisposable ? _onErrorChanged ;

    private TaskbarIcon ? _taskbarIcon ;

    [ ObservableProperty ]
    private ObservableCollection < MenuItem > _trayMenuItems = // todo do we need this or does this replace the current notifyicon?
        [new( ) { Header = "Home" , Tag = "tray_home" }] ;

    public MainWindowViewModel ( ISettingsManager manager ,
                                 IUiDeskManager   uiDeskManager )
    {
        Guard.ArgumentNotNull ( manager ,
                                nameof ( manager ) ) ;
        Guard.ArgumentNotNull ( uiDeskManager ,
                                nameof ( uiDeskManager ) ) ;

        _uiDeskManager = uiDeskManager ;

        SitViewItem.MouseDoubleClick        += OnClickSitViewItem ;
        StandViewItem.MouseDoubleClick      += OnClickStandViewItem ;
        ConnectViewItem.MouseDoubleClick    += OnClickConnectViewItem ;
        DisconnectViewItem.MouseDoubleClick += OnClickDisconnectViewItem ;
    }

    public void Dispose ( )
    {
        _logger?.Information ( "Disposing..." ) ;

        _uiDeskManager.Disconnect ( ) ;

        _uiDeskManager.Dispose ( ) ;
        _taskbarIcon?.Dispose ( ) ;
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

                                                       await _uiDeskManager.Sit ( ) ;
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

                                                       await _uiDeskManager.Stand ( ) ;
                                                   } ) ;
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

                                                       await _uiDeskManager.AutoConnect ( ) ;
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

                                                       await _uiDeskManager.Disconnect ( ) ;
                                                   } ) ;
    }

    public MainWindowViewModel Initialize ( IContainer container , TaskbarIcon taskbarIcon )
    {
        Guard.ArgumentNotNull ( container ,
                                nameof ( container ) ) ;
        Guard.ArgumentNotNull ( taskbarIcon ,
                                nameof ( taskbarIcon ) ) ;

        _taskbarIcon = taskbarIcon ;

        _logger = container.Resolve < ILogger > ( ) ;
        _logger?.Debug ( $"{nameof ( MainWindowViewModel )}: Initializing..." ) ;

        _uiDeskManager.Initialize ( container , taskbarIcon ) ;

        return this ;
    }
}