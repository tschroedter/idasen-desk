using System.Diagnostics ;

namespace Idasen.SystemTray.Win11.Interfaces ;

public interface IProcessLauncher
{
    /// <summary>
    ///     Starts a process using the provided start info.
    ///     Returns <see langword="true" /> if the process was started, <see langword="false" /> otherwise.
    /// </summary>
    bool TryStart ( ProcessStartInfo startInfo ) ;

    /// <summary>
    ///     Starts a process and waits for it to exit within the given timeout.
    /// </summary>
    /// <param name="startInfo">The process configuration. Must not be <see langword="null" />.</param>
    /// <param name="timeoutMs">The wait timeout in milliseconds; -1 waits indefinitely, and other values must be non-negative.</param>
    /// <returns>
    ///     <c>(false, null)</c> when the process could not be started;
    ///     <c>(true, null)</c> when the process started but did not complete within <paramref name="timeoutMs" />;
    ///     <c>(true, exitCode)</c> when the process completed.
    /// </returns>
    ( bool started , int ? exitCode ) StartAndWait ( ProcessStartInfo startInfo ,
                                                     int timeoutMs ) ;
}
