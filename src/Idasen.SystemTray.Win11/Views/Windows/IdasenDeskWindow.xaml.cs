using System.ComponentModel ;
using System.Diagnostics.CodeAnalysis ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.ViewModels.Windows ;
using JetBrains.Annotations ;
using Wpf.Ui ;
using Wpf.Ui.Abstractions ;
using Wpf.Ui.Appearance ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Windows ;

[ ExcludeFromCodeCoverage ]
public partial class IdasenDeskWindow : INavigationWindow , IMainWindow, IDisposable
{
    private readonly Subject < Visibility > _visibilityChanged = new( ) ;

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

        // Publish the initial visibility state
        _visibilityChanged.OnNext ( Visibility ) ;
    }

    public IObservable < Visibility > VisibilityChanged => _visibilityChanged.AsObservable ( ) ;

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
            _visibilityChanged.OnNext ( Visibility ) ; // Publish visibility change
            return ;
        }

        // Show and bring to foreground
        ShowInTaskbar = true ;
        Visibility    = Visibility.Visible ;

        _visibilityChanged.OnNext ( Visibility ) ;

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
        Visibility    = Visibility.Hidden ;
        _visibilityChanged.OnNext ( Visibility ) ;
        Hide ( ) ;
    }

    public void Dispose ( )
    {
        _visibilityChanged.OnCompleted ( ) ;
        _visibilityChanged.Dispose ( ) ;

        GC.SuppressFinalize ( this ) ;
    }
}