namespace Idasen.SystemTray.Win11.Interfaces ;

/// <summary>
///     Defines a strategy for reconnecting to the Bluetooth desk with retry logic.
/// </summary>
public interface IBluetoothReconnectStrategy
{
    /// <summary>
    ///     Attempts to connect to the desk with automatic retry using exponential backoff.
    /// </summary>
    /// <param name="connectAction">The function that performs the connection attempt.</param>
    /// <param name="cancellationToken">Cancellation token to stop retry attempts.</param>
    /// <returns>True if connection was successful, false otherwise.</returns>
    Task < bool > ConnectWithRetryAsync ( Func < CancellationToken , Task < bool > > connectAction ,
                                          CancellationToken                            cancellationToken ) ;

    /// <summary>
    ///     Resets the retry counter to start fresh attempts.
    /// </summary>
    void Reset ( ) ;

    /// <summary>
    ///     Gets the current retry attempt number (0-based).
    /// </summary>
    int CurrentAttempt { get ; }

    /// <summary>
    ///     Gets the maximum number of retry attempts before giving up.
    /// </summary>
    int MaxRetries { get ; }

    /// <summary>
    ///     Gets whether the strategy is currently in a retry cycle.
    /// </summary>
    bool IsRetrying { get ; }
}
