using JetBrains.Annotations ;

namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Monitors the health of an active Bluetooth connection and triggers reconnection when stale.
/// </summary>
public interface IBluetoothConnectionMonitor : IDisposable
{
    /// <summary>
    ///     Gets whether connection monitoring is currently active.
    /// </summary>
    [UsedImplicitly]
    bool IsMonitoring { get ; }

    /// <summary>
    ///     Event raised when a stale connection is detected and reconnection should be attempted.
    ///     The event is raised on the timer callback thread; handlers that require a specific
    ///     synchronization context, such as the WPF dispatcher, must marshal to it themselves.
    ///     Exceptions from a handler are logged and do not prevent other handlers from running.
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
