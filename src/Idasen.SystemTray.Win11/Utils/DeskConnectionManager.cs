using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public sealed class DeskConnectionManager : IDeskConnectionManager
{
    private readonly ILogger                       _logger ;
    private readonly ISettingsManager              _settingsManager ;
    private readonly Func < IDeskProvider >        _providerFactory ;
    private readonly IBluetoothReconnectStrategy   _reconnectStrategy ;
    private readonly IErrorManager                 _errorManager ;
    private readonly IBluetoothConnectionMonitor   _connectionMonitor ;

    private IDesk ?         _desk ;
    private IDeskProvider ? _deskProvider ;
    private bool            _disposed ;
    private bool            _isReconnecting ;

    public DeskConnectionManager (
        ILogger                      logger ,
        ISettingsManager             settingsManager ,
        Func < IDeskProvider >       providerFactory ,
        IBluetoothReconnectStrategy  reconnectStrategy ,
        IErrorManager                errorManager ,
        IBluetoothConnectionMonitor  connectionMonitor )
    {
        ArgumentNullException.ThrowIfNull ( logger ) ;
        ArgumentNullException.ThrowIfNull ( settingsManager ) ;
        ArgumentNullException.ThrowIfNull ( providerFactory ) ;
        ArgumentNullException.ThrowIfNull ( reconnectStrategy ) ;
        ArgumentNullException.ThrowIfNull ( errorManager ) ;
        ArgumentNullException.ThrowIfNull ( connectionMonitor ) ;

        _logger            = logger ;
        _settingsManager   = settingsManager ;
        _providerFactory   = providerFactory ;
        _reconnectStrategy = reconnectStrategy ;
        _errorManager      = errorManager ;
        _connectionMonitor = connectionMonitor ;

        // Subscribe to stale connection detection
        _connectionMonitor.StaleConnectionDetected += OnStaleConnectionDetected ;
    }

    public bool IsConnected => _desk is not null ;

    public IDesk ? CurrentDesk => _desk ;

    public IBluetoothConnectionMonitor ? ConnectionMonitor => _connectionMonitor ;

    public event EventHandler ? Connected ;
    public event EventHandler ? Disconnected ;
    public event EventHandler < IDesk > ? DeskReady ;

    public async Task ConnectAsync ( CancellationToken cancellationToken )
    {
        try
        {
            _logger.Debug ( "Initiating Bluetooth connection with automatic retry..." ) ;

            // Use reconnect strategy for automatic retry with exponential backoff
            var success = await _reconnectStrategy.ConnectWithRetryAsync (
                async token =>
                {
                    try
                    {
                        _logger.Debug ( "Attempting connection to desk..." ) ;

                        _deskProvider?.Dispose ( ) ;
                        _deskProvider = _providerFactory ( ) ;
                        _deskProvider.Initialize ( _settingsManager.CurrentSettings.DeviceSettings.DeviceName ,
                                                   _settingsManager.CurrentSettings.DeviceSettings.DeviceAddress ,
                                                   _settingsManager.CurrentSettings.DeviceSettings.DeviceMonitoringTimeout ) ;

                        var deviceName = _settingsManager.CurrentSettings.DeviceSettings.DeviceName ;
                        _logger.Debug ( "[{DeviceName}] Trying to connect to Idasen Desk..." ,
                                       deviceName ) ;

                        var (isSuccess , desk) = await _deskProvider.TryGetDesk ( token ).ConfigureAwait ( false ) ;

                        if ( isSuccess && desk != null )
                        {
                            HandleConnectionSuccessful ( desk ) ;
                            return true ;
                        }

                        _logger.Warning ( "Connection attempt failed - desk not found or unavailable" ) ;
                        return false ;
                    }
                    catch ( Exception e )
                    {
                        var deviceName = _desk?.DeviceName ?? _settingsManager.CurrentSettings.DeviceSettings.DeviceName ;
                        _logger.Warning ( e ,
                                         "[{DeviceName}] Connection attempt failed with exception" ,
                                         deviceName ) ;
                        return false ;
                    }
                } ,
                cancellationToken ).ConfigureAwait ( false ) ;

            if ( ! success )
            {
                HandleConnectionFailed ( ) ;
            }
        }
        catch ( Exception e )
        {
            var deviceName = _desk?.DeviceName ?? "Unknown" ;
            _logger.Error ( e ,
                           "[{DeviceName}] Fatal error during connection sequence" ,
                           deviceName ) ;
            HandleConnectionFailed ( ) ;
        }
    }

    public Task DisconnectAsync ( )
    {
        if ( ! IsConnected )
            return Task.CompletedTask ;

        try
        {
            _logger.Debug ( "[{DeviceName}] Trying to disconnect from Idasen Desk..." ,
                            _desk?.DeviceName ) ;

            // Stop connection monitoring when intentionally disconnecting
            _connectionMonitor.StopMonitoring ( ) ;

            DisposeDesk ( ) ;

            _logger.Debug ( "...disconnected from Idasen Desk" ) ;

            Disconnected?.Invoke ( this , EventArgs.Empty ) ;
        }
        catch ( Exception e )
        {
            _logger.Error ( e ,
                            "Failed to disconnect" ) ;
            HandleConnectionFailed ( ) ;
        }

        return Task.CompletedTask ;
    }

    public void Dispose ( )
    {
        if ( _disposed )
            return ;

        _disposed = true ;

        _logger.Information ( "Disposing {TypeName}..." ,
                              nameof ( DeskConnectionManager ) ) ;

        try
        {
            // Unsubscribe from connection monitor events
            _connectionMonitor.StaleConnectionDetected -= OnStaleConnectionDetected ;
            _connectionMonitor.Dispose ( ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to dispose connection monitor" ) ;
        }

        try
        {
            DisposeDesk ( ) ;
        }
        catch ( Exception ex )
        {
            _logger.Warning ( ex ,
                             "Failed to dispose desk during {TypeName} disposal" ,
                             nameof ( DeskConnectionManager ) ) ;
        }
    }

    private void HandleConnectionSuccessful ( IDesk desk )
    {
        _logger.Information ( "[{DeviceName}] Connected with address {BluetoothAddress}" ,
                              desk.DeviceName ,
                              desk.BluetoothAddress ) ;

        _desk = desk ;

        // Start connection monitoring after successful connection
        _connectionMonitor.StartMonitoring ( ) ;

        Connected?.Invoke ( this , EventArgs.Empty ) ;
        DeskReady?.Invoke ( this , desk ) ;

        // Apply device lock if configured (fire-and-forget with logging)
        if ( _settingsManager.CurrentSettings.DeviceSettings.DeviceLocked )
        {
            _logger.Information ( "Locking desk movement" ) ;
            ApplyDeviceLockInBackground ( desk ) ;
        }
    }

    private void ApplyDeviceLockInBackground ( IDesk desk )
    {
        Task.Run ( async ( ) =>
        {
            try
            {
                await desk.MoveLockAsync ( ) ;
                _logger.Debug ( "Device lock applied successfully" ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to apply device lock" ) ;
            }
        } ) ;
    }

    private void HandleConnectionFailed ( )
    {
        _logger.Debug ( "Connection failed..." ) ;

        // Disconnect in background with error handling
        Task.Run ( async ( ) =>
        {
            try
            {
                await DisconnectAsync ( ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Error during disconnect after connection failure" ) ;
            }
        } ) ;

        _errorManager.PublishForMessage ( "Failed to connect" ) ;
    }

    private void DisposeDesk ( )
    {
        _logger.Debug ( "[{DeskName}] Disposing desk" ,
                        _desk?.Name ) ;

        _desk?.Dispose ( ) ;
        _deskProvider?.Dispose ( ) ;

        _desk         = null ;
        _deskProvider = null ;
    }

    private void OnStaleConnectionDetected ( object ? sender , EventArgs e )
    {
        if ( _isReconnecting )
        {
            _logger.Debug ( "Stale connection event ignored - reconnection already in progress" ) ;
            return ;
        }

        _logger.Warning ( "Stale connection detected - initiating automatic reconnection..." ) ;

        // Handle reconnection in background to avoid blocking
        Task.Run ( async ( ) =>
        {
            try
            {
                _isReconnecting = true ;

                // Cleanup stale connection first
                try
                {
                    DisposeDesk ( ) ;
                }
                catch ( Exception ex )
                {
                    _logger.Warning ( ex ,
                                     "Error disposing stale desk connection" ) ;
                }

                // Notify disconnection
                Disconnected?.Invoke ( this , EventArgs.Empty ) ;

                // Attempt reconnection
                _logger.Information ( "Attempting automatic reconnection after stale connection..." ) ;
                await ConnectAsync ( CancellationToken.None ).ConfigureAwait ( false ) ;
            }
            catch ( Exception ex )
            {
                _logger.Error ( ex ,
                               "Failed to reconnect after stale connection detection" ) ;
                _errorManager.PublishForMessage ( "Failed to reconnect - connection was stale" ) ;
            }
            finally
            {
                _isReconnecting = false ;
            }
        } ) ;
    }
}
