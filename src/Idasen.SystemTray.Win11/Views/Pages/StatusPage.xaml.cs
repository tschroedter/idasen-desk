using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

/// <summary>
///     Interaction logic for StatusPage.xaml
/// </summary>
[ ExcludeFromCodeCoverage ]
public partial class StatusPage : INavigableView < StatusViewModel >
{
    public StatusPage ( StatusViewModel viewModel )
    {
        ViewModel   = viewModel ;
        DataContext = this ;

        InitializeComponent ( ) ;
    }

    public StatusViewModel ViewModel { get ; }
}