using System.Windows.Controls;
using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages
{
    /// <summary>
    /// Interaction logic for SitPage.xaml
    /// </summary>
    public partial class SitPage : INavigableView<SitViewModel>
    {
        public SitViewModel ViewModel { get; }

        public SitPage(SitViewModel viewModel)
        {
            ViewModel   = viewModel ;
            DataContext = this;

            InitializeComponent();
        }
    }
}
