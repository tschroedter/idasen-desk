using System.Diagnostics.CodeAnalysis ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class SettingsPage : INavigableView < SettingsViewModel >
{
    public SettingsPage ( SettingsViewModel viewModel )
    {
        ViewModel   = viewModel ;
        DataContext = this ;

        InitializeComponent ( ) ;

        PreviewMouseWheel += OnPreviewMouseWheel ;
    }

    public SettingsViewModel ViewModel { get ; }

    private void OnPreviewMouseWheel ( object sender , MouseWheelEventArgs e )
    {
        if ( e . Handled )
        {
            return ;
        }

        e . Handled = true ;

        var eventArg = new MouseWheelEventArgs ( e . MouseDevice , e . Timestamp , e . Delta )
        {
            RoutedEvent = UIElement . MouseWheelEvent ,
            Source = MainScrollViewer
        } ;

        MainScrollViewer . RaiseEvent ( eventArg ) ;
    }
}