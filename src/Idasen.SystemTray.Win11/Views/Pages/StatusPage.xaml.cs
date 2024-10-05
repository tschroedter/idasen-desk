using Idasen.SystemTray.Win11.ViewModels.Pages ;
using Wpf.Ui.Controls ;

namespace Idasen.SystemTray.Win11.Views.Pages
{
    /// <summary>
    /// Interaction logic for StatusPage.xaml
    /// </summary>
    public partial class StatusPage : INavigableView<StatusViewModel>
    {
        public StatusViewModel ViewModel { get; }

        public StatusPage(StatusViewModel viewModel)
        {
            ViewModel   = viewModel ;
            DataContext = this;

            InitializeComponent();
        }
    }
}
