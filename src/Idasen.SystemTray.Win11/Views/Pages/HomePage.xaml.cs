using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;
using System.Diagnostics;
using System.Windows.Navigation;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class HomePage : INavigableView < DashboardViewModel >
{
    public HomePage ( DashboardViewModel viewModel )
    {
        ViewModel   = viewModel ;
        DataContext = this ;

        InitializeComponent ( ) ;
    }

    public DashboardViewModel ViewModel { get ; }

    private void Hyperlink_RequestNavigate ( object sender , RequestNavigateEventArgs e )
    {
        try
        {
            Process.Start ( new ProcessStartInfo ( e.Uri.AbsoluteUri )
            {
                UseShellExecute = true
            } ) ;
        }
        catch
        {
            // Ignored
        }

        e.Handled = true ;
    }
}