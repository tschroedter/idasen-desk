using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Bluetooth ;
using Idasen.TestLogger ;

namespace Idasen.SystemTray.Win11.Tests.Utils.Bluetooth ;

public class BluetoothConnectionMonitorTests : IDisposable
{
    private const int TimeoutMillisecondsLong = 70000 ;

    private readonly InMemoryLogger _logger = new( ) ;

    private bool _disposed ;

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

    [ Fact ]
    public void Constructor_ValidParameters_CreatesInstance ( )
    {
        // Arrange & Act
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;

        // Assert
        monitor.Should ( ).NotBeNull ( ) ;
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void Constructor_NullLogger_ThrowsArgumentNullException ( )
    {
        // Arrange & Act
        var act = ( ) => new BluetoothConnectionMonitor ( null! ,
                                                          TimeoutMillisecondsLong ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( )
           .WithParameterName ( "logger" ) ;
    }

    [ Fact ]
    public void Constructor_ZeroTimeout_ThrowsArgumentOutOfRangeException ( )
    {
        // Arrange & Act
        var act = ( ) => new BluetoothConnectionMonitor ( _logger ,
                                                          0 ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( "timeoutMilliseconds" ) ;
    }

    [ Fact ]
    public void Constructor_NegativeTimeout_ThrowsArgumentOutOfRangeException ( )
    {
        // Arrange & Act
        var act = ( ) => new BluetoothConnectionMonitor ( _logger ,
                                                          - 1000 ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( "timeoutMilliseconds" ) ;
    }

    [ Fact ]
    public void StartMonitoring_WhenNotMonitoring_StartsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;

        // Act
        monitor.StartMonitoring ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StartMonitoring_WhenAlreadyMonitoring_RemainsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;
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
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;
        monitor.Dispose ( ) ;

        // Act
        var act = monitor.StartMonitoring ;

        // Assert
        act.Should ( ).Throw < ObjectDisposedException > ( ) ;
    }

    [ Fact ]
    public void StopMonitoring_WhenMonitoring_StopsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;
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
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;

        // Act
        monitor.StopMonitoring ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void ResetActivityTimer_WhenNotMonitoring_DoesNothing ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;

        // Act (should not throw)
        monitor.ResetActivityTimer ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void ResetActivityTimer_WhenMonitoring_DoesNotThrow ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;
        monitor.StartMonitoring ( ) ;

        // Act (should not throw)
        monitor.ResetActivityTimer ( ) ;

        // Assert
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void StaleConnectionDetected_AtTimeout_RaisesEvent ( )
    {
        // Arrange
        var timeProvider = new ManualTimeProvider ( ) ;
        var monitor      = new BluetoothConnectionMonitor ( _logger ,
                                                            100 ,
                                                            timeProvider ) ;
        var eventRaised = false ;
        monitor.StaleConnectionDetected += ( _ , _ ) => { eventRaised = true ; } ;
        monitor.StartMonitoring ( ) ;

        // Act: A callback that arrives early must wait until the full timeout has elapsed.
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 99 ) ) ;
        timeProvider.Timer.Fire ( ) ;
        eventRaised.Should ( ).BeFalse ( ) ;
        timeProvider.Timer.DueTime.Should ( ).Be ( TimeSpan.FromMilliseconds ( 1 ) ) ;

        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 1 ) ) ;
        timeProvider.Timer.Fire ( ) ;

        // Assert
        eventRaised.Should ( ).BeTrue ( ) ;
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void StaleConnectionDetected_AfterActivity_WaitsForNewTimeout ( )
    {
        // Arrange
        var timeProvider = new ManualTimeProvider ( ) ;
        var monitor      = new BluetoothConnectionMonitor ( _logger ,
                                                            200 ,
                                                            timeProvider ) ;
        var eventRaised = false ;
        monitor.StaleConnectionDetected += ( _ , _ ) => { eventRaised = true ; } ;
        monitor.StartMonitoring ( ) ;

        // Act
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 100 ) ) ;
        monitor.ResetActivityTimer ( ) ;
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 199 ) ) ;
        timeProvider.Timer.Fire ( ) ;

        // Assert
        eventRaised.Should ( ).BeFalse ( ) ;
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
        timeProvider.Timer.DueTime.Should ( ).Be ( TimeSpan.FromMilliseconds ( 1 ) ) ;

        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 1 ) ) ;
        timeProvider.Timer.Fire ( ) ;
        eventRaised.Should ( ).BeTrue ( ) ;
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void StaleConnectionDetected_WhenQueuedCallbackRunsAfterActivity_WaitsForFullTimeout ( )
    {
        // Arrange
        var timeProvider = new ManualTimeProvider ( ) ;
        var monitor      = new BluetoothConnectionMonitor ( _logger ,
                                                            100 ,
                                                            timeProvider ) ;
        var eventRaised = false ;
        monitor.StaleConnectionDetected += ( _ , _ ) => { eventRaised = true ; } ;
        monitor.StartMonitoring ( ) ;
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 75 ) ) ;
        monitor.ResetActivityTimer ( ) ;

        // Act: Simulate an elapsed callback queued before the activity reset.
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 50 ) ) ;
        timeProvider.Timer.Fire ( ) ;

        // Assert: The queued callback must not report stale activity early.
        eventRaised.Should ( ).BeFalse ( ) ;
        monitor.IsMonitoring.Should ( ).BeTrue ( ) ;
        timeProvider.Timer.DueTime.Should ( ).Be ( TimeSpan.FromMilliseconds ( 50 ) ) ;

        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 49 ) ) ;
        timeProvider.Timer.Fire ( ) ;
        eventRaised.Should ( ).BeFalse ( ) ;

        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 1 ) ) ;
        timeProvider.Timer.Fire ( ) ;
        eventRaised.Should ( ).BeTrue ( ) ;
        monitor.IsMonitoring.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public void StaleConnectionDetected_WhenHandlerThrows_LogsAndInvokesRemainingHandlers ( )
    {
        // Arrange
        var timeProvider = new ManualTimeProvider ( ) ;
        var monitor      = new BluetoothConnectionMonitor ( _logger ,
                                                            100 ,
                                                            timeProvider ) ;
        var secondHandlerInvoked = false ;
        monitor.StaleConnectionDetected += ( _ , _ ) => throw new InvalidOperationException ( "Handler failure" ) ;
        monitor.StaleConnectionDetected += ( _ , _ ) => secondHandlerInvoked = true ;
        monitor.StartMonitoring ( ) ;

        // Act
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 100 ) ) ;
        timeProvider.Timer.Fire ( ) ;

        // Assert
        secondHandlerInvoked.Should ( ).BeTrue ( ) ;
        _logger.Contains ( "Error while handling stale connection event" ,
                           Serilog.Events.LogEventLevel.Error )
                .Should ( ).BeTrue ( ) ;
    }

    [ Fact ]
    public void Dispose_WhenMonitoring_StopsMonitoring ( )
    {
        // Arrange
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;
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
        var monitor = new BluetoothConnectionMonitor ( _logger ,
                                                       TimeoutMillisecondsLong ) ;

        // Act
        var act = ( ) =>
                  {
#pragma warning disable S3966
                      monitor.Dispose ( ) ;
                      monitor.Dispose ( ) ;
#pragma warning restore S3966
                  } ;

        // Assert (should not throw)
        act.Should ( ).NotThrow ( ) ;
    }

    [ Fact ]
    public void StaleConnectionDetected_AfterDispose_DoesNotRaiseEvent ( )
    {
        // Arrange
        var timeProvider = new ManualTimeProvider ( ) ;
        var monitor      = new BluetoothConnectionMonitor ( _logger ,
                                                            100 ,
                                                            timeProvider ) ;
        var eventRaised = false ;
        monitor.StaleConnectionDetected += ( _ , _ ) => { eventRaised = true ; } ;
        monitor.StartMonitoring ( ) ;
        monitor.Dispose ( ) ;

        // Act: Simulate an elapsed callback that was queued before disposal.
        timeProvider.Advance ( TimeSpan.FromMilliseconds ( 100 ) ) ;
        timeProvider.Timer.Fire ( ) ;

        // Assert
        eventRaised.Should ( ).BeFalse ( ) ;
    }

    private sealed class ManualTimeProvider : TimeProvider
    {
        private long _timestamp ;

        public ManualTimer Timer { get ; private set ; } = null! ;

        public override long TimestampFrequency => TimeSpan.TicksPerSecond ;

        public override long GetTimestamp ( ) => _timestamp ;

        public override ITimer CreateTimer (
            TimerCallback callback ,
            object?       state ,
            TimeSpan      dueTime ,
            TimeSpan      period )
        {
            Timer = new ManualTimer ( callback , state ) ;
            Timer.Change ( dueTime , period ) ;
            return Timer ;
        }

        public void Advance ( TimeSpan elapsed ) => _timestamp += elapsed.Ticks ;
    }

    private sealed class ManualTimer (
        TimerCallback callback ,
        object?       state ) : ITimer
    {
        public TimeSpan DueTime { get ; private set ; }

        public bool Change ( TimeSpan dueTime , TimeSpan period )
        {
            DueTime = dueTime ;
            return true ;
        }

        public void Fire ( ) => callback ( state ) ;

        public void Dispose ( ) => DueTime = Timeout.InfiniteTimeSpan ;

        public ValueTask DisposeAsync ( )
        {
            Dispose ( ) ;
            return ValueTask.CompletedTask ;
        }
    }
}
