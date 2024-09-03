using System.Collections.ObjectModel ;
using Idasen.SystemTray.Win11.Views.Pages ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public partial class MainWindowViewModel : ObservableObject
{
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
        }
    ] ;

    [ ObservableProperty ]
    private ObservableCollection < MenuItem > _trayMenuItems =
        [new MenuItem { Header = "Home" , Tag = "tray_home" }] ;
}