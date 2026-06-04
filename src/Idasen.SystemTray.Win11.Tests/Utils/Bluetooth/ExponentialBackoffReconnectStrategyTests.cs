using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Bluetooth ;
using NSubstitute ;
using Serilog ;
using Xunit ;

namespace Idasen.SystemTray.Win11.Tests.Utils.Bluetooth ;

public class ExponentialBackoffReconnectStrategyTests
{
    private readonly ILogger _logger = Substitute.For < ILogger > ( ) ;

    [ Fact ]
    public async Task ConnectWithRetryAsync_SuccessOnFirstAttempt_ReturnsTrue ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( true ) ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
        strategy.CurrentAttempt.Should ( ).Be ( 0 ) ;
        await connectAction.Received ( 1 ).Invoke ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_FailsThenSucceeds_ReturnsTrue ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 3 ,
                                                                 initialDelayMs: 10 ) ; // Short delay for tests
        var callCount = 0 ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        connectAction ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( _ =>
                      {
                          callCount++ ;
                          return Task.FromResult ( callCount == 2 ) ; // Succeed on second attempt
                      } ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
        strategy.CurrentAttempt.Should ( ).Be ( 1 ) ; // First retry (second overall attempt)
        await connectAction.Received ( 2 ).Invoke ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_AllAttemptsFail_ReturnsFalse ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 2 ,
                                                                 initialDelayMs: 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
        strategy.CurrentAttempt.Should ( ).BeGreaterThan ( 0 ) ; // Retries were exhausted
        await connectAction.Received ( 3 ).Invoke ( Arg.Any < CancellationToken > ( ) ) ; // Initial + 2 retries
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_CancellationRequested_StopsRetrying ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 5 ,
                                                                 initialDelayMs: 100 ) ;
        using var cts = new CancellationTokenSource ( ) ;
        var callCount = 0 ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        connectAction ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( _ =>
                      {
                          callCount++ ;
                          if ( callCount == 2 ) cts.Cancel ( ) ; // Cancel after second attempt
                          return Task.FromResult ( false ) ;
                      } ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction , cts.Token ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
        callCount.Should ( ).BeLessThan ( 6 ) ; // Should stop before all retries
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_ExceptionThrown_ReturnsFalse ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 2 ,
                                                                 initialDelayMs: 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) )
                     .Returns < Task < bool > > ( _ => throw new InvalidOperationException ( "Connection failed" ) ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_NullAction_ThrowsArgumentNullException ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ) ;

        // Act
        Func < Task > act = async ( ) => await strategy.ConnectWithRetryAsync ( null! , CancellationToken.None ) ;

        // Assert
        await act.Should ( ).ThrowAsync < ArgumentNullException > ( ) ;
    }

    [ Fact ]
    public void Constructor_NullLogger_ThrowsArgumentNullException ( )
    {
        // Act
        Action act = ( ) => _ = new ExponentialBackoffReconnectStrategy ( null! ) ;

        // Assert
        act.Should ( ).Throw < ArgumentNullException > ( ) ;
    }

    [ Fact ]
    public void MaxRetries_ReturnsConfiguredValue ( )
    {
        // Arrange
        const int expectedMaxRetries = 7 ;
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger , maxRetries: expectedMaxRetries ) ;

        // Act & Assert
        strategy.MaxRetries.Should ( ).Be ( expectedMaxRetries ) ;
    }

    [ Fact ]
    public async Task Reset_ClearsState ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 2 ,
                                                                 initialDelayMs: 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;
        var attemptsBefore = strategy.CurrentAttempt ;

        strategy.Reset ( ) ;

        // Assert
        attemptsBefore.Should ( ).BeGreaterThan ( 0 ) ;
        strategy.CurrentAttempt.Should ( ).Be ( 0 ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_UsesExponentialBackoff ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 3 ,
                                                                 initialDelayMs: 100 ,
                                                                 backoffMultiplier: 2.0 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        var startTime = DateTime.UtcNow ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        var elapsed = DateTime.UtcNow - startTime ;

        // Assert
        // Expected delays: 100ms, 200ms, 400ms = 700ms total
        // With some tolerance for execution time
        elapsed.TotalMilliseconds.Should ( ).BeGreaterThan ( 650 ) ;
        elapsed.TotalMilliseconds.Should ( ).BeLessThan ( 1500 ) ; // Allow some overhead
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_CapsDelayAtMaximum ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 5 ,
                                                                 initialDelayMs: 1000 ,
                                                                 maxDelayMs: 2000 ,
                                                                 backoffMultiplier: 2.0 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        var startTime = DateTime.UtcNow ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        var elapsed = DateTime.UtcNow - startTime ;

        // Assert
        // Expected delays with cap:
        //   Attempt 1: 1000ms
        //   Attempt 2: 2000ms (capped, would be 2000ms)
        //   Attempt 3: 2000ms (capped, would be 4000ms)
        //   Attempt 4: 2000ms (capped, would be 8000ms)
        //   Attempt 5: 2000ms (capped, would be 16000ms)
        // Total: 9000ms

        elapsed.TotalMilliseconds.Should ( ).BeGreaterThan ( 8500 ) ;
        elapsed.TotalMilliseconds.Should ( ).BeLessThan ( 12000 ) ; // Allow overhead
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_MultipleCallsInSequence_ResetsState ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 1 ,
                                                                 initialDelayMs: 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        // First call fails
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;
        await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;
        var attemptAfterFirst = strategy.CurrentAttempt ;

        // Second call succeeds
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( true ) ) ;
        await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        // Assert
        attemptAfterFirst.Should ( ).BeGreaterThan ( 0 ) ; // Had retries after first call
        strategy.CurrentAttempt.Should ( ).Be ( 0 ) ; // Reset to 0 on new successful call
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task IsRetrying_IsTrueDuringRetry ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 maxRetries: 2 ,
                                                                 initialDelayMs: 100 ) ;
        var isRetryingDuringExecution = false ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        connectAction ( Arg.Any < CancellationToken > ( ) )
                     .Returns ( async _ =>
                      {
                          await Task.Delay ( 10 ) ;
                          isRetryingDuringExecution = strategy.IsRetrying ;
                          return false ;
                      } ) ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction , CancellationToken.None ) ;

        // Assert
        isRetryingDuringExecution.Should ( ).BeTrue ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ; // After completion
    }
}
