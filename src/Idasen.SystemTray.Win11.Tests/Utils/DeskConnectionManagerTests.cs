using FluentAssertions ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.SystemTray.Win11.Interfaces ;
using Idasen.SystemTray.Win11.TraySettings ;
using Idasen.SystemTray.Win11.Utils ;
using Idasen.TestLogger ;
using NSubstitute ;
using NSubstitute.ExceptionExtensions ;

namespace Idasen.SystemTray.Win11.Tests.Utils ;

public class DeskConnectionManagerTests : IDisposable
{
    private readonly IBluetoothConnectionMonitor _connectionMonitor ;
    private readonly IDesk                       _desk ;
    private readonly IDeskProvider               _deskProvider ;
    private readonly IErrorManager               _errorManager ;
    private readonly InMemoryLogger              _logger ;
    private readonly Func < IDeskProvider >      _providerFactory ;
    private readonly IBluetoothReconnectStrategy _reconnectStrategy ;
    private readonly ISettingsManager            _settingsManager ;
    private          bool                        _disposed ;

    public DeskConnectionManagerTests ( )
    {
        _logger            = new InMemoryLogger ( ) ;
        _settingsManager   = Substitute.For < ISettingsManager > ( ) ;
        _reconnectStrategy = Substitute.For < IBluetoothReconnectStrategy > ( ) ;
        _errorManager      = Substitute.For < IErrorManager > ( ) ;
        _connectionMonitor = Substitute.For < IBluetoothConnectionMonitor > ( ) ;
        _deskProvider      = Substitute.For < IDeskProvider > ( ) ;
        _desk              = Substitute.For < IDesk > ( ) ;

        // Setup default device settings
        var settings = CreateDefaultSettings ( ) ;
        _settingsManager.CurrentSettings.Returns ( settings ) ;

        _providerFactory = ( ) => _deskProvider ;
    }

    public void Dispose ( )
    {
        Dispose ( true ) ;

        GC.SuppressFinalize ( this ) ;
    }

    protected virtual void Dispose ( bool disposing )
    {
        if ( _disposed )
            return ;

        if ( disposing ) _logger.Dispose ( ) ;

        _disposed = true ;
    }

    private static Settings CreateDefaultSettings ( )
    {
        return new Settings
        {
            DeviceSettings = new DeviceSettings
            {
                DeviceName              = "TestDesk" ,
                DeviceAddress           = 123456789 ,
                DeviceMonitoringTimeout = 600 ,
                DeviceLocked            = false
            }
        } ;
    }

    [ Fact ]
    public void Constructor_ValidParameters_CreatesInstance ( )
    {
        // Act
        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Assert
        manager.Should ( ).NotBeNull ( ) ;
        manager.IsConnected.Should ( ).BeFalse ( ) ;
        manager.CurrentDesk.Should ( ).BeNull ( ) ;
        manager.ConnectionMonitor.Should ( ).Be ( _connectionMonitor ) ;
    }

    [ Fact ]
    public void Constructor_NullLogger_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskConnectionManager (
                                                    null! ,
                                                    _settingsManager ,
                                                    _providerFactory ,
                                                    _reconnectStrategy ,
                                                    _errorManager ,
                                                    _connectionMonitor ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void Constructor_NullSettingsManager_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskConnectionManager (
                                                    _logger ,
                                                    null! ,
                                                    _providerFactory ,
                                                    _reconnectStrategy ,
                                                    _errorManager ,
                                                    _connectionMonitor ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "settingsManager" ) ;
    }

    [ Fact ]
    public void Constructor_NullProviderFactory_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskConnectionManager (
                                                    _logger ,
                                                    _settingsManager ,
                                                    null! ,
                                                    _reconnectStrategy ,
                                                    _errorManager ,
                                                    _connectionMonitor ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "providerFactory" ) ;
    }

    [ Fact ]
    public void Constructor_NullReconnectStrategy_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskConnectionManager (
                                                    _logger ,
                                                    _settingsManager ,
                                                    _providerFactory ,
                                                    null! ,
                                                    _errorManager ,
                                                    _connectionMonitor ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "reconnectStrategy" ) ;
    }

    [ Fact ]
    public void Constructor_NullErrorManager_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskConnectionManager (
                                                    _logger ,
                                                    _settingsManager ,
                                                    _providerFactory ,
                                                    _reconnectStrategy ,
                                                    null! ,
                                                    _connectionMonitor ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "errorManager" ) ;
    }

    [ Fact ]
    public void Constructor_NullConnectionMonitor_ThrowsArgumentNullException ( )
    {
        // Act
        var act = ( ) => new DeskConnectionManager (
                                                    _logger ,
                                                    _settingsManager ,
                                                    _providerFactory ,
                                                    _reconnectStrategy ,
                                                    _errorManager ,
                                                    null! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "connectionMonitor" ) ;
    }

    [ Fact ]
    public void Constructor_SubscribesToStaleConnectionEvent ( )
    {
        // Act
        _ = new DeskConnectionManager (
                                       _logger ,
                                       _settingsManager ,
                                       _providerFactory ,
                                       _reconnectStrategy ,
                                       _errorManager ,
                                       _connectionMonitor ) ;

        // Assert
        _connectionMonitor.Received ( 1 ).StaleConnectionDetected += Arg.Any < EventHandler > ( ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_SuccessfulConnection_SetsIsConnectedTrue ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Assert
        manager.IsConnected.Should ( ).BeTrue ( ) ;
        manager.CurrentDesk.Should ( ).Be ( _desk ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_SuccessfulConnection_RaisesConnectedEvent ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        var connectedEventRaised = false ;
        manager.Connected += ( _ , _ ) => connectedEventRaised = true ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Assert
        connectedEventRaised.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_SuccessfulConnection_RaisesDeskReadyEvent ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        IDesk ? deskFromEvent = null ;
        manager.DeskReady += ( _ , desk ) => deskFromEvent = desk ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Assert
        deskFromEvent.Should ( ).Be ( _desk ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_SuccessfulConnection_StartsMonitoring ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Assert
        _connectionMonitor.Received ( 1 ).StartMonitoring ( ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_SuccessfulConnectionWithDeviceLocked_AppliesLock ( )
    {
        // Arrange
        var settings = CreateDefaultSettings ( ) ;
        settings.DeviceSettings.DeviceLocked = true ;
        _settingsManager.CurrentSettings.Returns ( settings ) ;

        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;
        await Task.Delay ( 100 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background task

        // Assert
        await _desk.Received ( 1 ).MoveLockAsync ( ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_FailedConnection_CallsHandleConnectionFailed ( )
    {
        // Arrange
        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( Task.FromResult ( false ) ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;
        await Task.Delay ( 100 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background task

        // Assert
        _errorManager.Received ( 1 ).PublishForMessage ( "Failed to connect" ,
                                                         Arg.Any < string > ( ) ) ;
        manager.IsConnected.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_ExceptionDuringConnection_HandlesGracefully ( )
    {
        // Arrange
        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Throws ( new InvalidOperationException ( "Test exception" ) ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;
        await Task.Delay ( 100 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background task

        // Assert
        manager.IsConnected.Should ( ).BeFalse ( ) ;
        _errorManager.Received ( 1 ).PublishForMessage ( "Failed to connect" ,
                                                         Arg.Any < string > ( ) ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_TryGetDeskReturnsNull_ReturnsFalse ( )
    {
        // Arrange
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( false , ( IDesk ? )null ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Assert
        manager.IsConnected.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task ConnectAsync_TryGetDeskThrowsException_ReturnsFalse ( )
    {
        // Arrange
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Throws ( new InvalidOperationException ( "Test exception" ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Assert
        manager.IsConnected.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_WhenNotConnected_ReturnsImmediately ( )
    {
        // Arrange
        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        await manager.DisconnectAsync ( ) ;

        // Assert
        _connectionMonitor.DidNotReceive ( ).StopMonitoring ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_WhenConnected_DisconnectsSuccessfully ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Act
        await manager.DisconnectAsync ( ) ;

        // Assert
        manager.IsConnected.Should ( ).BeFalse ( ) ;
        manager.CurrentDesk.Should ( ).BeNull ( ) ;
        _connectionMonitor.Received ( 1 ).StopMonitoring ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_WhenConnected_RaisesDisconnectedEvent ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        var disconnectedEventRaised = false ;
        manager.Disconnected += ( _ , _ ) => disconnectedEventRaised = true ;

        // Act
        await manager.DisconnectAsync ( ) ;

        // Assert
        disconnectedEventRaised.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task DisconnectAsync_ExceptionDuringDisconnect_DoesNotThrow ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _desk.When ( x => x.Dispose ( ) ).Do ( _ => throw new InvalidOperationException ( "Test exception" ) ) ;

        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Act & Assert - should not throw
        var act = async ( ) => await manager.DisconnectAsync ( ) ;
        await act.Should ( ).NotThrowAsync ( ) ;
    }

    [ Fact ]
    public void Dispose_WhenNotDisposed_DisposesResources ( )
    {
        // Arrange
        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        manager.Dispose ( ) ;

        // Assert
        _connectionMonitor.Received ( 1 ).StaleConnectionDetected -= Arg.Any < EventHandler > ( ) ;
        _connectionMonitor.Received ( 1 ).Dispose ( ) ;
    }

    [ Fact ]
    public void Dispose_MultipleTimes_DoesNotThrow ( )
    {
        // Arrange
        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        var act = ( ) =>
                  {
#pragma warning disable S3966
                      manager.Dispose ( ) ;
                      manager.Dispose ( ) ;
#pragma warning restore S3966
                  } ;

        // Assert
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public async Task Dispose_WhenConnected_DisposesDesk ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Act
        manager.Dispose ( ) ;

        // Assert
        _desk.Received ( 1 ).Dispose ( ) ;
        _deskProvider.Received ( ).Dispose ( ) ;
    }

    [ Fact ]
    public void Dispose_ExceptionInConnectionMonitorDispose_HandlesGracefully ( )
    {
        // Arrange
        _connectionMonitor.When ( x => x.Dispose ( ) )
                          .Do ( _ => throw new InvalidOperationException ( "Test exception" ) ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act
        var act = manager.Dispose ;

        // Assert - should not throw
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public async Task Dispose_ExceptionInDeskDispose_HandlesGracefully ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _desk.When ( x => x.Dispose ( ) ).Do ( _ => throw new InvalidOperationException ( "Test exception" ) ) ;

        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Act
        var act = manager.Dispose ;

        // Assert - should not throw
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public async Task OnStaleConnectionDetected_TriggersReconnection ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        var disconnectedEventRaised = false ;
        manager.Disconnected += ( _ , _ ) => disconnectedEventRaised = true ;

        // Act
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;
        await Task.Delay ( 200 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background task

        // Assert
        disconnectedEventRaised.Should ( ).BeTrue ( ) ;
        await _reconnectStrategy.Received ( 2 ).ConnectWithRetryAsync (
                                                                       Arg.Any < Func < CancellationToken ,
                                                                           Task < bool > > > ( ) ,
                                                                       Arg.Any <
                                                                           CancellationToken > ( ) ) ; // Once for initial connect, once for reconnect
    }

    [ Fact ]
    public async Task OnStaleConnectionDetected_WhileReconnecting_IgnoresEvent ( )
    {
        // Arrange
        var reconnectCallCount = 0 ;

        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( async callInfo =>
                                     {
                                         reconnectCallCount ++ ;
                                         if ( reconnectCallCount == 1 )
                                             // First call waits to simulate long reconnection
                                             await Task.Delay ( 300 ,
                                                                TestContext.Current.CancellationToken ) ;
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return await connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        _ = new DeskConnectionManager (
                                       _logger ,
                                       _settingsManager ,
                                       _providerFactory ,
                                       _reconnectStrategy ,
                                       _errorManager ,
                                       _connectionMonitor ) ;

        // Act - Trigger first event, then rapidly trigger more while first is still reconnecting
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;
        await Task.Delay ( 50 ,
                           TestContext.Current.CancellationToken ) ; // Let first reconnection start
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;

        await Task.Delay ( 500 ,
                           TestContext.Current.CancellationToken ) ; // Wait for reconnection to complete

        // Assert - should only reconnect once (the other 2 events were ignored)
        reconnectCallCount.Should ( ).Be ( 1 ) ;
    }

    [ Fact ]
    public async Task OnStaleConnectionDetected_ReconnectionFails_DoesNotThrow ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( false , ( IDesk ? )null ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        // Act - Trigger stale connection event which will attempt reconnection
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;
        await Task.Delay ( 300 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background reconnection attempt

        // Assert - should handle gracefully, publishing error message
        _errorManager.Received ( 1 ).PublishForMessage ( "Failed to connect" ,
                                                         Arg.Any < string > ( ) ) ;
        manager.IsConnected.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task OnStaleConnectionDetected_WhenAlreadyConnected_DoesNotReconnect ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;
        _deskProvider.ConnectionStatus.Returns ( Windows.Devices.Bluetooth.BluetoothConnectionStatus.Connected ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        var disconnectedEventRaised = false ;
        manager.Disconnected += ( _ , _ ) => disconnectedEventRaised = true ;

        _connectionMonitor.ClearReceivedCalls();

        // Act
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;
        await Task.Delay ( 200 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background task

        // Assert
        disconnectedEventRaised.Should ( ).BeFalse ( ) ;
        _connectionMonitor.Received ( 1 ).StartMonitoring ( ) ; // Should restart monitoring since connection is still valid
        await _reconnectStrategy.Received ( 1 ).ConnectWithRetryAsync (
                                                                       Arg.Any < Func < CancellationToken ,
                                                                           Task < bool > > > ( ) ,
                                                                       Arg.Any <
                                                                           CancellationToken > ( ) ) ; // Only once for initial connect, not for reconnect
    }

    [ Fact ]
    public async Task OnStaleConnectionDetected_DisposeDeskThrows_HandlesGracefully ( )
    {
        // Arrange
        _desk.DeviceName.Returns ( "TestDesk" ) ;
        _desk.BluetoothAddress.Returns ( 123456789UL ) ;
        _desk.When ( x => x.Dispose ( ) ).Do ( _ => throw new InvalidOperationException ( "Test exception" ) ) ;

        _deskProvider.TryGetDesk ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( Task.FromResult ( ( true , ( IDesk ? )_desk ) ) ) ;

        _reconnectStrategy.ConnectWithRetryAsync (
                                                  Arg.Any < Func < CancellationToken , Task < bool > > > ( ) ,
                                                  Arg.Any < CancellationToken > ( ) )
                          .Returns ( callInfo =>
                                     {
                                         var connectFunc =
                                             callInfo.Arg < Func < CancellationToken , Task < bool > > > ( ) ;
                                         return connectFunc ( CancellationToken.None ) ;
                                     } ) ;

        var manager = new DeskConnectionManager (
                                                 _logger ,
                                                 _settingsManager ,
                                                 _providerFactory ,
                                                 _reconnectStrategy ,
                                                 _errorManager ,
                                                 _connectionMonitor ) ;

        await manager.ConnectAsync ( CancellationToken.None ) ;

        // Act - should not throw
        _connectionMonitor.StaleConnectionDetected += Raise.Event < EventHandler > ( _connectionMonitor ,
                                                                                     EventArgs.Empty ) ;
        await Task.Delay ( 200 ,
                           TestContext.Current.CancellationToken ) ; // Wait for background task

        // Assert - should handle gracefully
        await _reconnectStrategy.Received ( 2 ).ConnectWithRetryAsync (
                                                                       Arg.Any < Func < CancellationToken ,
                                                                           Task < bool > > > ( ) ,
                                                                       Arg.Any < CancellationToken > ( ) ) ;
    }
}
