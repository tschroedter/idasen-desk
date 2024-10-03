using System.Diagnostics.Metrics;
using Idasen.SystemTray.Win11.ViewModels.Windows ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class SitViewModel : ObservableObject
{
    private readonly IUiDeskManager _deskManager ;

    public SitViewModel (IUiDeskManager deskManager)
    {
        _deskManager = deskManager ;

        NewSitCommand = new AsyncRelayCommand(NewSit);
    }

    [ObservableProperty]
    private string _title = "Moving desk...";

    [ObservableProperty]
    private string _status = "Done";

    [RelayCommand]
    private void OnSit()
    {
    }

    public IAsyncRelayCommand NewSitCommand { get; }

    private async Task NewSit()
    {
        await _deskManager.Stand().ConfigureAwait(false);
    }
}