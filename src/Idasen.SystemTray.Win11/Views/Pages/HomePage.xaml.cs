using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Microsoft.Extensions.Configuration ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class HomePage : INavigableView < DashboardViewModel >
{
    private readonly IConfiguration _configuration ;

    public HomePage ( DashboardViewModel viewModel , IConfiguration configuration )
    {
        ViewModel      = viewModel ;
        DataContext    = this ;
        _configuration = configuration ;

        InitializeComponent ( ) ;
    }

    public DashboardViewModel ViewModel { get ; }

    private void DonateImage_OnClick ( object sender , MouseButtonEventArgs e )
    {
        try
        {
            var donateUrl = _configuration [ "DonateUrl" ] ?? string.Empty ;

            if ( string.IsNullOrEmpty ( donateUrl ) )
                return ;

            var uri = new Uri ( donateUrl ) ;

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