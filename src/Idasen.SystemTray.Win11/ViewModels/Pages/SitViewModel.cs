namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class SitViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Moving desk to sitting position...";

    [ObservableProperty]
    private uint _height;

    [ObservableProperty]
    private string _status = "Done";
}