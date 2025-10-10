using System.ComponentModel ;
using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.ViewModels.Windows ;
using JetBrains.Annotations ;
using Wpf.Ui ;
using Wpf.Ui.Abstractions ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;
using System.Windows.Controls; // Added for ScrollViewer
using System.Windows.Input; // Added for MouseWheelEventArgs
using System.Windows.Media; // Added for VisualTreeHelper

namespace Idasen.SystemTray.Win11.Views.Windows ;

[ ExcludeFromCodeCoverage ]
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

    public bool Navigate ( Type pageType )
    {
        return RootNavigation.Navigate ( pageType ) ;
    }

    public void SetServiceProvider ( IServiceProvider serviceProvider )
    {
        throw new NotImplementedException ( ) ;
    }

    public void SetPageService ( INavigationViewPageProvider navigationViewPageProvider )
    {
        RootNavigation.SetPageProviderService ( navigationViewPageProvider ) ;
    }

    public void ShowWindow ( )
    {
        ShowInTaskbar = true ;
        Show ( ) ;
        Activate ( ) ;
    }

    public void CloseWindow ( )
    {
        Close ( ) ;
    }

    private void IdasenNotifyIcon_OnLeftClick ( object sender , RoutedEventArgs e )
    {
        // Toggle: If visible -> hide; if hidden/minimized -> show and activate
        if ( Visibility == Visibility.Visible )
        {
            ShowInTaskbar = false ;
            Visibility    = Visibility.Hidden ;
            return ;
        }

        // Show and bring to foreground
        ShowInTaskbar = true ;
        Visibility    = Visibility.Visible ;
        if ( WindowState == WindowState.Minimized )
            WindowState = WindowState.Normal ;

        Activate ( ) ;
        Focus ( ) ;
        Topmost = true ;
        Topmost = false ;
    }

    [ UsedImplicitly ]
    public INavigationView GetNavigation ( )
    {
        return RootNavigation ;
    }

    protected override void OnInitialized ( EventArgs e )
    {
        base.OnInitialized ( e ) ;

        // Prevent duplicate subscriptions if re-initialized
        Closing -= OnWindowClosing ;
        Closing += OnWindowClosing ;
    }

    private void OnWindowClosing ( object ? sender , CancelEventArgs e )
    {
        // Cancel close and hide instead
        e.Cancel      = true ;
        ShowInTaskbar = false ;
        Hide ( ) ;
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        // Find the ScrollViewer within the NavigationView
        var scrollViewer = FindVisualChild<ScrollViewer>(RootNavigation);
        if (scrollViewer != null)
        {
            // Scroll vertically based on the mouse wheel delta
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta / 120.0);
        }
    }

    private static T? FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T typedChild)
            {
                return typedChild;
            }

            var descendant = FindVisualChild<T>(child);
            if (descendant != null)
            {
                return descendant;
            }
        }

        return null;
    }
}