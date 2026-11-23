using System.Diagnostics ;
using System.Diagnostics.CodeAnalysis ;
using Idasen.SystemTray.Win11.Interfaces ;

namespace Idasen.SystemTray.Win11.Services ;

/// <summary>
///     Thin wrapper over <see cref="System.Diagnostics.Process" /> so that process launching
///     can be substituted in unit tests.
/// </summary>
[ ExcludeFromCodeCoverage ] // Wraps OS process launching – verified through integration/manual testing
public sealed class ProcessLauncher : IProcessLauncher
{
    /// <inheritdoc />
    public bool TryStart ( ProcessStartInfo startInfo )
    {
        ArgumentNullException.ThrowIfNull ( startInfo ) ;

        using var process = Process.Start ( startInfo ) ;

        return process != null ;
    }

    /// <inheritdoc />
    public ( bool started , int ? exitCode ) StartAndWait ( ProcessStartInfo startInfo ,
                                                            int timeoutMs )
    {
        ArgumentNullException.ThrowIfNull ( startInfo ) ;

        if ( timeoutMs < Timeout.Infinite )
            throw new ArgumentOutOfRangeException ( nameof ( timeoutMs ) ,
                                                    timeoutMs ,
                                                    "Timeout must be -1 (infinite) or non-negative." ) ;

        using var process = Process.Start ( startInfo ) ;

        if ( process == null )
            return ( false , null ) ;

        if ( ! process.WaitForExit ( timeoutMs ) )
            return ( true , null ) ;

        return ( true , process.ExitCode ) ;
    }
}
