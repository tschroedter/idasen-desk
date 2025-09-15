using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;
using System.Diagnostics;
using System.Windows.Navigation;
using System.Windows.Input;

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

    private void DonateImage_OnClick ( object sender , MouseButtonEventArgs e )
    {
        try
        {
            var uri = new Uri ( "https://www.paypal.com/donate/?hosted_button_id=KAWJDNVJTD7SJ" ) ;

            Process.Start ( new ProcessStartInfo ( uri.AbsoluteUri )
            {
                UseShellExecute = true
            } ) ;
        }
        catch
        {
            // Ignored
        }
    }
}