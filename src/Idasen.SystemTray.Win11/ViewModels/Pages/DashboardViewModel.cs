using Idasen.BluetoothLE.Core ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.ViewModels.Pages ;

public partial class DashboardViewModel : ObservableObject
{
    [ ObservableProperty ]
    private string _title ;

    public DashboardViewModel ( IVersionProvider versionProvider )
    {
        Guard.ArgumentNotNull ( versionProvider ,
                                nameof ( versionProvider ) ) ;

        _title = "Idasen Desk Application " + versionProvider.GetVersion ( ) ;
    }
}