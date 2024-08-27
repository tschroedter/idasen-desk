using Idasen.SystemTray.Win11.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace Idasen.SystemTray.Win11.Views.Pages
{
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
    }
}
