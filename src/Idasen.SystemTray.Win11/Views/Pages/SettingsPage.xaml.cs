using Idasen.SystemTray.Win11.ViewModels.Pages ;
using System.Diagnostics.CodeAnalysis;
using Wpf.Ui.Abstractions.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages ;

[ExcludeFromCodeCoverage]
public partial class SettingsPage : INavigableView < SettingsViewModel >
{
    public SettingsPage ( SettingsViewModel viewModel )
    {
        ViewModel   = viewModel ;
        DataContext = this ;

        InitializeComponent ( ) ;
    }

    public SettingsViewModel ViewModel { get ; }
}