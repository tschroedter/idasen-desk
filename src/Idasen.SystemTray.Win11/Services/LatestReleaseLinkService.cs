using System.Diagnostics ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Services ;

public sealed class LatestReleaseLinkService ( IProcessLauncher processLauncher ) : ILatestReleaseLinkService
{
    private const string LatestReleaseUrl = "https://github.com/tschroedter/idasen-desk/releases/latest" ;

    public bool OpenLatestRelease ( )
    {
        var startInfo = new ProcessStartInfo
        {
            FileName        = LatestReleaseUrl ,
            UseShellExecute = true
        } ;

        return processLauncher.TryStart ( startInfo ) ;
    }
}
