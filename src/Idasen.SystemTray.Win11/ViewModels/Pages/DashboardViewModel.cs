namespace Idasen.SystemTray.Win11.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private int _counter ;

        [RelayCommand]
        private void OnCounterIncrement()
        {
            Counter++;
        }
    }
}
