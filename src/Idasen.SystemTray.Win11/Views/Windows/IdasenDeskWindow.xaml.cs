using Idasen.SystemTray.Win11.ViewModels.Windows ;
using JetBrains.Annotations ;
using Wpf.Ui ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Windows ;

public partial class IdasenDeskWindow : INavigationWindow
{
    public IdasenDeskWindow ( IdasenDeskWindowViewModel viewModel ,
                              IPageService              pageService ,
                              INavigationService        navigationService )
    {
        ViewModel   = viewModel ;
        DataContext = this ;

        SystemThemeWatcher.Watch ( this ) ;

        InitializeComponent ( ) ;
        SetPageService ( pageService ) ;

        navigationService.SetNavigationControl ( RootNavigation ) ;
    }

    public IdasenDeskWindowViewModel ViewModel { get ; }

    INavigationView INavigationWindow.GetNavigation ( )
    {
        throw new NotImplementedException ( ) ;
    }

    [ UsedImplicitly ]
    public INavigationView GetNavigation ( )
    {
        return RootNavigation ;
    }

    public bool Navigate ( Type pageType )
    {
        return RootNavigation.Navigate ( pageType ) ;
    }

    public void SetServiceProvider ( IServiceProvider serviceProvider )
    {
        throw new NotImplementedException ( ) ;
    }

    public void SetPageService ( IPageService pageService )
    {
        RootNavigation.SetPageService ( pageService ) ;
    }

    public void ShowWindow ( )
    {
        Show ( ) ;
    }

    public void CloseWindow ( )
    {
        Close ( ) ;
    }
}