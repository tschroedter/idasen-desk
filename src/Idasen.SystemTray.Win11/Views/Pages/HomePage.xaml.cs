using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

public partial class HomePage : INavigableView < DashboardViewModel >
{
    public HomePage ( DashboardViewModel viewModel )
    {
        ViewModel   = viewModel ;
        DataContext = this ;

        InitializeComponent ( ) ;
    }

    public DashboardViewModel ViewModel { get ; }
}