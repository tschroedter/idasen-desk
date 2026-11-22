using System.Windows ;
using System.Windows.Input ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.Utils ;
using Serilog ;

namespace Idasen.SystemTray.Win11.ViewModels.Windows ;

public partial class UpdateAvailableViewModel : ObservableObject , IDisposable
{
    private readonly ILogger _logger ;
    private readonly ILatestReleaseLinkService _latestReleaseLinkService ;
    private readonly IDisposable _updateSubscription ;

    private bool _disposed ;

    [ ObservableProperty ] private Visibility _updateAvailableVisibility ;

    public UpdateAvailableViewModel ( ILogger logger ,
                                      IUpdateAvailableState updateAvailableState ,
                                      ILatestReleaseLinkService latestReleaseLinkService )
    {
        _logger                    = logger ;
        _latestReleaseLinkService = latestReleaseLinkService ;
        OpenLatestReleaseCommand = new DelegateCommand ( OpenLatestRelease ) ;
        UpdateAvailableVisibility = updateAvailableState.IsUpdateAvailable
                                        ? Visibility.Visible
                                        : Visibility.Collapsed ;
        _updateSubscription = updateAvailableState.UpdateAvailableChanged.Subscribe ( SetUpdateAvailableVisibility ) ;
    }

    public ICommand OpenLatestReleaseCommand { get ; }

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _updateSubscription.Dispose ( ) ;
        _disposed = true ;
        GC.SuppressFinalize ( this ) ;
    }

    private void OpenLatestRelease ( object ? _ )
    {
        if ( ! _latestReleaseLinkService.OpenLatestRelease ( ) )
            _logger.Warning ( "Failed to open the latest Idasen Desk release website." ) ;
    }

    private void SetUpdateAvailableVisibility ( bool available )
    {
        var visibility = available ? Visibility.Visible : Visibility.Collapsed ;
        var dispatcher = Application.Current?.Dispatcher ;

        if ( dispatcher is null || dispatcher.CheckAccess ( ) )
            UpdateAvailableVisibility = visibility ;
        else
            dispatcher.BeginInvoke ( ( ) => UpdateAvailableVisibility = visibility ) ;
    }
}
