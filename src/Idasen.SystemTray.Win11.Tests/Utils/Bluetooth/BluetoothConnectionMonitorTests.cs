using FluentAssertions ;
using NSubstitute ;
using Serilog ;
using Idasen.SystemTray.Win11.Utils.Bluetooth ;
using Xunit.Abstractions ;

namespace Idasen.SystemTray.Win11.Tests.Utils.Bluetooth ;

public class BluetoothConnectionMonitorTests
{
    private readonly ILogger _logger ;

    public BluetoothConnectionMonitorTests ( )
    {
        _logger = Substitute.For < ILogger > ( ) ;
    }

    [ Fact ]
    public void Constructor_ValidParameters_CreatesInstance ( )
    {
        // Arrange & Act
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;

        // Assert
        monitor.Should ( ).NotBeNull ( ) ;
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void Constructor_NullLogger_ThrowsArgumentNullException ( )
    {
        // Arrange & Act
        var act = ( ) => new BluetoothConnectionMonitor ( null! , 60000 ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void Constructor_ZeroTimeout_ThrowsArgumentOutOfRangeException ( )
    {
        // Arrange & Act
        var act = ( ) => new BluetoothConnectionMonitor ( _logger , 0 ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( "timeoutMilliseconds" ) ;
    }

    [ Fact ]
    public void Constructor_NegativeTimeout_ThrowsArgumentOutOfRangeException ( )
    {
        // Arrange & Act
        var act = ( ) => new BluetoothConnectionMonitor ( _logger , - 1000 ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( "timeoutMilliseconds" ) ;
    }

    [ Fact ]
    public void StartMonitoring_WhenNotMonitoring_StartsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;

        // Act
        monitor.StartMonitoring ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StartMonitoring_WhenAlreadyMonitoring_RemainsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;
        monitor.StartMonitoring ( ) ;

        // Act
        monitor.StartMonitoring ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StartMonitoring_AfterDispose_ThrowsObjectDisposedException ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;
        monitor.Dispose ( ) ;

        // Act
        var act = ( ) => monitor.StartMonitoring ( ) ;

        // Assert
        act.Should ( ).Throw < ObjectDisposedException > ( ) ;
    }

    [ Fact ]
    public void StopMonitoring_WhenMonitoring_StopsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;
        monitor.StartMonitoring ( ) ;

        // Act
        monitor.StopMonitoring ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void StopMonitoring_WhenNotMonitoring_RemainsNotMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;

        // Act
        monitor.StopMonitoring ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void ResetActivityTimer_WhenNotMonitoring_DoesNothing ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;

        // Act (should not throw)
        monitor.ResetActivityTimer ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void ResetActivityTimer_WhenMonitoring_DoesNotThrow ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;
        monitor.StartMonitoring ( ) ;

        // Act (should not throw)
        monitor.ResetActivityTimer ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public async Task StaleConnectionDetected_AfterTimeout_RaisesEvent ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 100 ) ; // 100ms timeout for quick test
        var eventRaised = false ;

        monitor.StaleConnectionDetected += ( sender , args ) =>
        {
            eventRaised = true ;
        } ;

        // Act
        monitor.StartMonitoring ( ) ;
        await Task.Delay ( 200 , TestContext.Current.CancellationToken ) ; // Wait longer than timeout

        // Assert
        eventRaised.Should ( ).BeTrue ( ) ;
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ; // Should stop monitoring after event
    }

    [ Fact ]
    public async Task StaleConnectionDetected_WithActivityBeforeTimeout_DoesNotRaiseEvent ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 200 ) ; // 200ms timeout
        var eventRaised = false ;

        monitor.StaleConnectionDetected += ( sender , args ) =>
        {
            eventRaised = true ;
        } ;

        // Act
        monitor.StartMonitoring ( ) ;
        await Task.Delay ( 100 , TestContext.Current.CancellationToken ) ; // Wait half the timeout
        monitor.ResetActivityTimer ( ) ; // Reset timer before timeout
        await Task.Delay ( 150 , TestContext.Current.CancellationToken ) ; // Wait another 150ms (total 250ms, but timer was reset at 100ms)

        // Assert
        eventRaised.Should ( ).BeFalse ( ) ;
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ; // Should still be monitoring
    }

    [ Fact ]
    public void Dispose_WhenMonitoring_StopsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;
        monitor.StartMonitoring ( ) ;

        // Act
        monitor.Dispose ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void Dispose_MultipleTimes_DoesNotThrow ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 60000 ) ;

        // Act
        var act = ( ) =>
        {
            monitor.Dispose ( ) ;
            monitor.Dispose ( ) ;
        } ;

        // Assert (should not throw)
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public async Task StaleConnectionDetected_AfterDispose_DoesNotRaiseEvent ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger , 100 ) ; // 100ms timeout
        var eventRaised = false ;

        monitor.StaleConnectionDetected += ( sender , args ) =>
        {
            eventRaised = true ;
        } ;

        monitor.StartMonitoring ( ) ;

        // Act
        monitor.Dispose ( ) ;
        await Task.Delay ( 200 , TestContext.Current.CancellationToken ) ; // Wait longer than timeout

        // Assert
        eventRaised.Should ( ).BeFalse ( ) ;
    }
}
