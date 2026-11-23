using FluentAssertions ;
using Idasen.SystemTray.Win11.Utils.Bluetooth ;
using Idasen.TestLogger ;
using NSubstitute ;

namespace Idasen.SystemTray.Win11.Tests.Utils.Bluetooth ;

public class ExponentialBackoffReconnectStrategyTests : IDisposable
{
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
    public async Task ConnectWithRetryAsync_SuccessOnFirstAttempt_ReturnsTrue ( )
    {
        // Arrange
        var strategy      = new ExponentialBackoffReconnectStrategy ( _logger ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( true ) ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction ,
                                                            CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeTrue ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
        strategy.CurrentAttempt.Should ( ).Be ( 0 ) ;
        await connectAction.Received ( 1 ).Invoke ( Arg.Any < CancellationToken > ( ) ) ;
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_ConcurrentCalls_AreSerialized ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ) ;
        var firstConnectionStarted = new TaskCompletionSource ( TaskCreationOptions.RunContinuationsAsynchronously ) ;
        var completeFirstConnection = new TaskCompletionSource < bool > ( TaskCreationOptions.RunContinuationsAsynchronously ) ;
        var secondActionInvoked = false ;

        var firstCall = strategy.ConnectWithRetryAsync (
                                                        async _ =>
                                                        {
                                                            firstConnectionStarted.SetResult ( ) ;
                                                            return await completeFirstConnection.Task.ConfigureAwait ( false ) ;
                                                        } ,
                                                        CancellationToken.None ) ;
        await firstConnectionStarted.Task ;

        var secondCall = strategy.ConnectWithRetryAsync (
                                                         _ =>
                                                         {
                                                             secondActionInvoked = true ;
                                                             return Task.FromResult ( true ) ;
                                                         } ,
                                                         CancellationToken.None ) ;

        // Assert while the first connection is still pending.
        secondActionInvoked.Should ( ).BeFalse ( ) ;
        strategy.IsRetrying.Should ( ).BeTrue ( ) ;

        // Act
        completeFirstConnection.SetResult ( true ) ;
        var firstResult  = await firstCall ;
        var secondResult = await secondCall ;

        // Assert
        firstResult.Should ( ).BeTrue ( ) ;
        secondResult.Should ( ).BeTrue ( ) ;
        secondActionInvoked.Should ( ).BeTrue ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_FailsThenSucceeds_ReturnsTrue ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 3 ,
                                                                 10 ) ; // Short delay for tests
        var callCount     = 0 ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        connectAction ( Arg.Any < CancellationToken > ( ) )
           .Returns ( _ =>
                      {
                          callCount ++ ;
                          return Task.FromResult ( callCount == 2 ) ; // Succeed on second attempt
                      } ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction ,
                                                            CancellationToken.None ) ;

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
                                                                 2 ,
                                                                 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction ,
                                                            CancellationToken.None ) ;

        // Assert
        result.Should ( ).BeFalse ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
        strategy.CurrentAttempt.Should ( ).Be ( strategy.MaxRetries ) ;
        await connectAction.Received ( 3 ).Invoke ( Arg.Any < CancellationToken > ( ) ) ; // Initial + 2 retries
    }

    [ Fact ]
    public async Task ConnectWithRetryAsync_CancellationRequested_StopsRetrying ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 5 ,
                                                                 100 ) ;
        using var cts           = new CancellationTokenSource ( ) ;
        var       callCount     = 0 ;
        var       connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        connectAction ( Arg.Any < CancellationToken > ( ) )
           .Returns ( _ =>
                      {
                          callCount ++ ;
                          if ( callCount == 2 ) cts.Cancel ( ) ; // Cancel after second attempt
                          return Task.FromResult ( false ) ;
                      } ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction ,
                                                            cts.Token ) ;

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
                                                                 2 ,
                                                                 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) )
           .Returns < Task < bool > > ( _ => throw new InvalidOperationException ( "Connection failed" ) ) ;

        // Act
        var result = await strategy.ConnectWithRetryAsync ( connectAction ,
                                                            CancellationToken.None ) ;

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
        Func < Task > act = async ( ) => await strategy.ConnectWithRetryAsync ( null! ,
                                                                                CancellationToken.None ) ;

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

    [ Theory ]
    [ InlineData ( - 1 ) ]
    [ InlineData ( - 5 ) ]
    public void Constructor_NegativeMaxRetries_ThrowsArgumentOutOfRangeException ( int maxRetries )
    {
        // Act
        Action act = ( ) => _ = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                           maxRetries ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( nameof ( maxRetries ) ) ;
    }

    [ Theory ]
    [ InlineData ( 0 ) ]
    [ InlineData ( - 1 ) ]
    public void Constructor_NonpositiveInitialDelay_ThrowsArgumentOutOfRangeException ( int initialDelayMs )
    {
        // Act
        Action act = ( ) => _ = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                           initialDelayMs : initialDelayMs ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( nameof ( initialDelayMs ) ) ;
    }

    [ Theory ]
    [ InlineData ( 0 , 1000 ) ]
    [ InlineData ( - 1 , 1000 ) ]
    [ InlineData ( 999 , 1000 ) ]
    public void Constructor_InvalidMaximumDelay_ThrowsArgumentOutOfRangeException (
        int maxDelayMs ,
        int initialDelayMs )
    {
        // Act
        Action act = ( ) => _ = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                           initialDelayMs : initialDelayMs ,
                                                                           maxDelayMs : maxDelayMs ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( nameof ( maxDelayMs ) ) ;
    }

    [ Theory ]
    [ InlineData ( 0.5 ) ]
    [ InlineData ( 1.0 ) ]
    public void Constructor_NonIncreasingMultiplier_ThrowsArgumentOutOfRangeException ( double backoffMultiplier )
    {
        // Act
        Action act = ( ) => _ = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                           backoffMultiplier : backoffMultiplier ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( nameof ( backoffMultiplier ) ) ;
    }

    [ Fact ]
    public void Constructor_NonfiniteMultiplier_ThrowsArgumentOutOfRangeException ( )
    {
        // Act
        Action act = ( ) => _ = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                           backoffMultiplier : double.NaN ) ;

        // Assert
        act.Should ( ).Throw < ArgumentOutOfRangeException > ( )
           .WithParameterName ( "backoffMultiplier" ) ;
    }

    [ Fact ]
    public void MaxRetries_ReturnsConfiguredValue ( )
    {
        // Arrange
        const int expectedMaxRetries = 7 ;
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 expectedMaxRetries ) ;

        // Act & Assert
        strategy.MaxRetries.Should ( ).Be ( expectedMaxRetries ) ;
    }

    [ Fact ]
    public async Task Reset_ClearsState ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 2 ,
                                                                 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction ,
                                               CancellationToken.None ) ;
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
                                                                 3 ,
                                                                 100 ,
                                                                 backoffMultiplier : 2.0 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        var startTime = DateTime.UtcNow ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction ,
                                               CancellationToken.None ) ;

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
                                                                 5 ,
                                                                 1000 ,
                                                                 2000 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;

        var startTime = DateTime.UtcNow ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction ,
                                               CancellationToken.None ) ;

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
                                                                 1 ,
                                                                 10 ) ;
        var connectAction = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        // First call fails
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( false ) ) ;
        await strategy.ConnectWithRetryAsync ( connectAction ,
                                               CancellationToken.None ) ;
        var attemptAfterFirst = strategy.CurrentAttempt ;

        // Second call succeeds
        connectAction ( Arg.Any < CancellationToken > ( ) ).Returns ( Task.FromResult ( true ) ) ;
        await strategy.ConnectWithRetryAsync ( connectAction ,
                                               CancellationToken.None ) ;

        // Assert
        attemptAfterFirst.Should ( ).BeGreaterThan ( 0 ) ; // Had retries after first call
        strategy.CurrentAttempt.Should ( ).Be ( 0 ) ;      // Reset to 0 on new successful call
        strategy.IsRetrying.Should ( ).BeFalse ( ) ;
    }

    [ Fact ]
    public async Task IsRetrying_IsTrueDuringRetry ( )
    {
        // Arrange
        var strategy = new ExponentialBackoffReconnectStrategy ( _logger ,
                                                                 2 ,
                                                                 100 ) ;
        var isRetryingDuringExecution = false ;
        var connectAction             = Substitute.For < Func < CancellationToken , Task < bool > > > ( ) ;

        connectAction ( Arg.Any < CancellationToken > ( ) )
           .Returns ( async _ =>
                      {
                          await Task.Delay ( 10 ) ;
                          isRetryingDuringExecution = strategy.IsRetrying ;
                          return false ;
                      } ) ;

        // Act
        await strategy.ConnectWithRetryAsync ( connectAction ,
                                               CancellationToken.None ) ;

        // Assert
        isRetryingDuringExecution.Should ( ).BeTrue ( ) ;
        strategy.IsRetrying.Should ( ).BeFalse ( ) ; // After completion
    }
}
