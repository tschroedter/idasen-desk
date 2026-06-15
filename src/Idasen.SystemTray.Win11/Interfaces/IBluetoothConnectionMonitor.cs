namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Monitors the health of an active Bluetooth connection and triggers reconnection when stale.
/// </summary>
public interface IBluetoothConnectionMonitor : IDisposable
{
    /// <summary>
    ///     Gets whether connection monitoring is currently active.
    /// </summary>
    bool IsMonitoring { get ; }

    /// <summary>
    ///     Event raised when a stale connection is detected and reconnection should be attempted.
    /// </summary>
    event EventHandler ? StaleConnectionDetected ;

    /// <summary>
    ///     Starts monitoring the Bluetooth connection for activity.
    /// </summary>
    void StartMonitoring ( ) ;

    /// <summary>
    ///     Stops monitoring the Bluetooth connection.
    /// </summary>
    void StopMonitoring ( ) ;

    /// <summary>
    ///     Resets the activity timer, indicating the connection is alive and responsive.
    ///     Should be called when any desk activity is detected (height changes, commands, etc.).
    /// </summary>
    void ResetActivityTimer ( ) ;
}
