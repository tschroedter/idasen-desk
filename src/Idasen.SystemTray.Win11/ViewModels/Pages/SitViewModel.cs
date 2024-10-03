namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class SitViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Moving desk...";

    [ObservableProperty]
    private string _status = "Done";
}