using System.Timers ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;
using Timer = System.Timers.Timer ;

namespace Idasen.SystemTray.Win11.Utils.Bluetooth ;

/// <summary>
///     Monitors the health of an active Bluetooth connection using a watchdog timer pattern.
///     Detects stale connections by tracking time since last activity and triggers reconnection.
/// </summary>
public sealed class BluetoothConnectionMonitor : IBluetoothConnectionMonitor
{
    private readonly ILogger _logger ;
    private readonly int     _timeoutMilliseconds ;
    private readonly Timer   _watchdogTimer ;

    private bool _disposed ;
    private bool _isMonitoring ;

    /// <summary>
    ///     Creates a new instance of the connection monitor.
    /// </summary>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="timeoutMilliseconds">Timeout in milliseconds before connection is considered stale (default: 60000ms = 60s).</param>
    public BluetoothConnectionMonitor (
        ILogger logger ,
        int     timeoutMilliseconds = 60000 )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;

        if ( timeoutMilliseconds <= 0 )
            throw new ArgumentOutOfRangeException ( nameof ( timeoutMilliseconds ) ,
                                                   "Timeout must be greater than zero" ) ;

        _logger               = logger ;
        _timeoutMilliseconds  = timeoutMilliseconds ;

        // Create timer but don't start it yet
        _watchdogTimer          = new Timer ( _timeoutMilliseconds ) ;
        _watchdogTimer.Elapsed += OnWatchdogTimerElapsed ;
        _watchdogTimer.AutoReset = false ; // One-shot timer, reset manually on activity
    }

    public bool IsMonitoring => _isMonitoring ;

    public event EventHandler ? StaleConnectionDetected ;

    public void StartMonitoring ( )
    {
        ObjectDisposedException.ThrowIf ( _disposed , nameof ( BluetoothConnectionMonitor ) ) ;

        if ( _isMonitoring )
        {
            _logger.Debug ( "Connection monitoring already active" ) ;
            return ;
        }

        _logger.Information ( "Starting Bluetooth connection monitoring with timeout of {TimeoutSeconds}s" ,
                             _timeoutMilliseconds / 1000.0 ) ;

        _isMonitoring = true ;
        _watchdogTimer.Start ( ) ;
    }

    public void StopMonitoring ( )
    {
        if ( ! _isMonitoring )
            return ;

        _logger.Information ( "Stopping Bluetooth connection monitoring" ) ;

        _isMonitoring = false ;
        _watchdogTimer.Stop ( ) ;
    }

    public void ResetActivityTimer ( )
    {
        if ( ! _isMonitoring )
            return ;

        _logger.Debug ( "Connection activity detected - resetting watchdog timer" ) ;

        // Stop and restart the timer to reset the countdown
        _watchdogTimer.Stop ( ) ;
        _watchdogTimer.Start ( ) ;
    }

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( BluetoothConnectionMonitor ) ) ;

        StopMonitoring ( ) ;

        try
        {
            _watchdogTimer.Elapsed -= OnWatchdogTimerElapsed ;
            _watchdogTimer.Dispose ( ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to dispose watchdog timer" ) ;
        }
    }

    private void OnWatchdogTimerElapsed ( object ? sender , ElapsedEventArgs e )
    {
        if ( ! _isMonitoring )
            return ;

        _logger.Warning ( "Stale Bluetooth connection detected - no activity for {TimeoutSeconds}s" ,
                         _timeoutMilliseconds / 1000.0 ) ;

        // Stop monitoring before raising event to prevent multiple triggers
        StopMonitoring ( ) ;

        try
        {
            StaleConnectionDetected?.Invoke ( this , EventArgs.Empty ) ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                           "Error while handling stale connection event" ) ;
        }
    }
}
