using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Services ;

/// <summary>
///     Thin wrapper over <see cref="System.Diagnostics.Process" /> so that process launching
///     can be substituted in unit tests.
/// </summary>
[ ExcludeFromCodeCoverage ] // Wraps OS process launching – verified through integration/manual testing
public class ProcessLauncher : IProcessLauncher
{
    /// <inheritdoc />
    public bool TryStart ( ProcessStartInfo startInfo )
    {
        using var process = Process.Start ( startInfo ) ;

        return process != null ;
    }

    /// <inheritdoc />
    public ( bool started , int? exitCode ) StartAndWait ( ProcessStartInfo startInfo , int timeoutMs )
    {
        using var process = Process.Start ( startInfo ) ;

        if ( process == null )
            return ( false , null ) ;

        if ( ! process.WaitForExit ( timeoutMs ) )
            return ( true , null ) ;

        return ( true , process.ExitCode ) ;
    }
}
