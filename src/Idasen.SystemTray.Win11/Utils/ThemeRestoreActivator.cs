using System.Diagnostics.CodeAnalysis ;
using Microsoft.Extensions.Hosting ;

namespace Idasen.SystemTray.Win11.Utils ;

[ ExcludeFromCodeCoverage ]
public class ThemeRestoreActivator : IHostedService
{
    // The constructor injects ThemeRestoreOnResume to ensure it's created during host start.
    public ThemeRestoreActivator ( ThemeRestoreOnResume themeRestoreOnResume )
    {
        themeRestoreOnResume.Initialize ( ) ;
    }

    public Task StartAsync ( CancellationToken cancellationToken )
    {
        return Task.CompletedTask ;
    }

    public Task StopAsync ( CancellationToken cancellationToken )
    {
        return Task.CompletedTask ;
    }
}