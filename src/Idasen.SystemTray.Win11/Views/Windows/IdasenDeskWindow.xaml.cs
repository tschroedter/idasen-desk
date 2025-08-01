﻿using Idasen.SystemTray.Win11.ViewModels.Windows ;
using JetBrains.Annotations ;
using System.Diagnostics.CodeAnalysis;
using Wpf.Ui ;
using Wpf.Ui.Abstractions ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Windows ;

[ExcludeFromCodeCoverage]
public partial class IdasenDeskWindow : INavigationWindow
{
    public IdasenDeskWindow ( IdasenDeskWindowViewModel   viewModel ,
                              INavigationViewPageProvider pageService ,
                              INavigationService          navigationService )
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

    public void SetPageService (INavigationViewPageProvider pageService )
    {
        RootNavigation.SetPageProviderService ( pageService ) ;
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