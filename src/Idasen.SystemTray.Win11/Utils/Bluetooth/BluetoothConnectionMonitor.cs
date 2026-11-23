using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Bluetooth ;

/// <summary>
///     Monitors the health of an active Bluetooth connection using a watchdog timer pattern.
///     Detects stale connections by tracking time since last activity and triggers reconnection.
/// </summary>
public sealed class BluetoothConnectionMonitor : IBluetoothConnectionMonitor
{
    private readonly object        _syncRoot = new( ) ;
    private readonly ILogger       _logger ;
    private readonly TimeProvider  _timeProvider ;
    private readonly TimeSpan      _timeout ;
    private readonly int           _timeoutMilliseconds ;
    private readonly ITimer        _watchdogTimer ;

    private bool _disposed ;
    private bool _isMonitoring ;
    private long _lastActivityTimestamp ;

    /// <summary>
    ///     Creates a new instance of the connection monitor.
    /// </summary>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="timeoutMilliseconds">
    ///     Timeout in milliseconds before connection is considered stale (default: 60000ms =
    ///     60s).
    /// </param>
    public BluetoothConnectionMonitor (
        ILogger      logger ,
        int          timeoutMilliseconds = 60000 ,
        TimeProvider? timeProvider = null )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;

        if ( timeoutMilliseconds <= 0 )
            throw new ArgumentOutOfRangeException ( nameof ( timeoutMilliseconds ) ,
                                                    "Timeout must be greater than zero" ) ;

        _logger              = logger ;
        _timeProvider        = timeProvider ?? TimeProvider.System ;
        _timeout             = TimeSpan.FromMilliseconds ( timeoutMilliseconds ) ;
        _timeoutMilliseconds = timeoutMilliseconds ;
        _watchdogTimer       = _timeProvider.CreateTimer ( OnWatchdogTimerElapsed ,
                                                           null ,
                                                           Timeout.InfiniteTimeSpan ,
                                                           Timeout.InfiniteTimeSpan ) ;
    }

    public bool IsMonitoring
    {
        get
        {
            lock ( _syncRoot )
                return _isMonitoring ;
        }
    }

    public event EventHandler ? StaleConnectionDetected ;

    public void StartMonitoring ( )
    {
        lock ( _syncRoot )
        {
            ObjectDisposedException.ThrowIf ( _disposed ,
                                              nameof ( BluetoothConnectionMonitor ) ) ;

            if ( _isMonitoring )
            {
                _logger.Debug ( "Connection monitoring already active" ) ;
                return ;
            }

            _isMonitoring          = true ;
            _lastActivityTimestamp = _timeProvider.GetTimestamp ( ) ;
            _watchdogTimer.Change ( _timeout ,
                                    Timeout.InfiniteTimeSpan ) ;
        }

        _logger.Debug ( "Starting Bluetooth connection monitoring with timeout of {TimeoutSeconds}s" ,
                        _timeoutMilliseconds / 1000.0 ) ;
    }

    public void StopMonitoring ( )
    {
        lock ( _syncRoot )
        {
            if ( ! _isMonitoring )
                return ;

            _isMonitoring = false ;
            _watchdogTimer.Change ( Timeout.InfiniteTimeSpan ,
                                    Timeout.InfiniteTimeSpan ) ;
        }

        _logger.Debug ( "Stopping Bluetooth connection monitoring" ) ;
    }

    public void ResetActivityTimer ( )
    {
        lock ( _syncRoot )
        {
            if ( ! _isMonitoring )
                return ;

            _lastActivityTimestamp = _timeProvider.GetTimestamp ( ) ;
            _watchdogTimer.Change ( _timeout ,
                                    Timeout.InfiniteTimeSpan ) ;
        }

        _logger.Debug ( "Connection activity detected - resetting watchdog timer" ) ;
    }

    public void Dispose ( )
    {
        lock ( _syncRoot )
        {
            if ( _disposed )
                return ;

            _disposed      = true ;
            _isMonitoring = false ;
            _watchdogTimer.Dispose ( ) ;
        }

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( BluetoothConnectionMonitor ) ) ;
    }

    private void OnWatchdogTimerElapsed ( object? state )
    {
        lock ( _syncRoot )
        {
            if ( _disposed || ! _isMonitoring )
                return ;

            var elapsed = _timeProvider.GetElapsedTime ( _lastActivityTimestamp ) ;
            if ( elapsed < _timeout )
            {
                _watchdogTimer.Change ( _timeout - elapsed ,
                                        Timeout.InfiniteTimeSpan ) ;
                return ;
            }

            _isMonitoring = false ;
        }

        _logger.Debug ( "Stale Bluetooth connection detected - no activity for {TimeoutSeconds}s" ,
                        _timeoutMilliseconds / 1000.0 ) ;

        try
        {
            StaleConnectionDetected?.Invoke ( this ,
                                              EventArgs.Empty ) ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                            "Error while handling stale connection event" ) ;
        }
    }
}
