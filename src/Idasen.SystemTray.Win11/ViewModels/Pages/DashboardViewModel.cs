using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.BluetoothLE.Core;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class DashboardViewModel : ObservableObject
{
    public DashboardViewModel ( IVersionProvider versionProvider)
    {
        Guard.ArgumentNotNull(versionProvider,
                              nameof(versionProvider));

        _title = "Idasen Desk Application " + versionProvider.GetVersion (  ) ;
    }

    [ ObservableProperty ]
    private string _title ;
}