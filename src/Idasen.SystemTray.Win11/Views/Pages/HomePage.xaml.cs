using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;

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