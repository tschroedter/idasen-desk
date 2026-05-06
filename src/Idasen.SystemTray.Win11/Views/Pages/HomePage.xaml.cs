using System.Diagnostics.CodeAnalysis ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ ExcludeFromCodeCoverage ]
public partial class HomePage : INavigableView < DashboardViewModel >
{
    private readonly IDonateService _donateService ;

    public HomePage ( DashboardViewModel viewModel ,
                      IDonateService     donateService )
    {
        ViewModel      = viewModel ;
        DataContext    = this ;
        _donateService = donateService ;

        InitializeComponent ( ) ;
    }

    public DashboardViewModel ViewModel { get ; }

    private void DonateImage_OnClick ( object sender , MouseButtonEventArgs e )
    {
        _donateService.OpenDonateUrl ( ) ;
    }
}