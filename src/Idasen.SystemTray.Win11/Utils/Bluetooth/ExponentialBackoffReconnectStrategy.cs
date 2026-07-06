using Idasen.SystemTray.Win11.Interfaces ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils.Bluetooth ;

/// <summary>
///     Implements exponential backoff strategy for Bluetooth reconnection attempts.
/// </summary>
public class ExponentialBackoffReconnectStrategy : IBluetoothReconnectStrategy
{
    private readonly double  _backoffMultiplier ;
    private readonly int     _initialDelayMs ;
    private readonly ILogger _logger ;
    private readonly int     _maxDelayMs ;

    /// <summary>
    ///     Creates a new instance with default exponential backoff parameters.
    /// </summary>
    /// <param name="logger">Logger for diagnostics.</param>
    /// <param name="maxRetries">Maximum number of retry attempts (default: 5).</param>
    /// <param name="initialDelayMs">Initial delay in milliseconds (default: 1000ms).</param>
    /// <param name="maxDelayMs">Maximum delay cap in milliseconds (default: 30000ms = 30s).</param>
    /// <param name="backoffMultiplier">Multiplier for exponential growth (default: 2.0).</param>
    public ExponentialBackoffReconnectStrategy (
        ILogger logger ,
        int     maxRetries        = 5 ,
        int     initialDelayMs    = 1000 ,
        int     maxDelayMs        = 30000 ,
        double  backoffMultiplier = 2.0 )
    {
        _logger            = logger ?? throw new ArgumentNullException ( nameof ( logger ) ) ;
        MaxRetries         = maxRetries ;
        _initialDelayMs    = initialDelayMs ;
        _maxDelayMs        = maxDelayMs ;
        _backoffMultiplier = backoffMultiplier ;
        CurrentAttempt     = 0 ;
        IsRetrying         = false ;
    }

    public int CurrentAttempt { get ; private set ; }

    public int MaxRetries { get ; }

    public bool IsRetrying { get ; private set ; }

    public async Task < bool > ConnectWithRetryAsync (
        Func < CancellationToken , Task < bool > > connectAction ,
        CancellationToken                          cancellationToken )
    {
        ArgumentNullException.ThrowIfNull ( connectAction ) ;

        IsRetrying     = true ;
        CurrentAttempt = 0 ;

        try
        {
            // Try initial connection
            _logger.Information ( "Attempting initial Bluetooth connection..." ) ;

            var success = await connectAction ( cancellationToken ).ConfigureAwait ( false ) ;

            if ( success )
            {
                _logger.Information ( "Bluetooth connection successful on initial attempt" ) ;
                return true ;
            }

            // Start retry loop with exponential backoff
            _logger.Warning ( "Initial connection failed. Starting retry sequence with exponential backoff..." ) ;

            for ( CurrentAttempt = 1 ; CurrentAttempt <= MaxRetries ; CurrentAttempt ++ )
            {
                // Check for cancellation
                if ( cancellationToken.IsCancellationRequested )
                {
                    _logger.Information ( "Retry sequence cancelled by user" ) ;
                    return false ;
                }

                // Calculate delay with exponential backoff
                var delay = CalculateDelay ( CurrentAttempt ) ;

                _logger.Information (
                                     "Retry attempt {Attempt}/{MaxRetries} - waiting {DelaySeconds:F1} seconds before next attempt" ,
                                     CurrentAttempt ,
                                     MaxRetries ,
                                     delay / 1000.0 ) ;

                try
                {
                    await Task.Delay ( delay ,
                                       cancellationToken ).ConfigureAwait ( false ) ;
                }
                catch ( TaskCanceledException ex )
                {
                    _logger.Information ( ex ,
                                          "Delay cancelled during retry sequence" ) ;
                    return false ;
                }

                // Attempt connection
                _logger.Information ( "Attempting Bluetooth connection (retry {Attempt}/{MaxRetries})..." ,
                                      CurrentAttempt ,
                                      MaxRetries ) ;

                success = await connectAction ( cancellationToken ).ConfigureAwait ( false ) ;

                if ( success )
                {
                    _logger.Information ( "Bluetooth connection successful on retry attempt {Attempt}" ,
                                          CurrentAttempt ) ;
                    return true ;
                }

                _logger.Warning ( "Retry attempt {Attempt}/{MaxRetries} failed" ,
                                  CurrentAttempt ,
                                  MaxRetries ) ;
            }

            // All retries exhausted
            _logger.Error ( "All {MaxRetries} retry attempts exhausted. Connection failed." ,
                            MaxRetries ) ;

            return false ;
        }
        catch ( Exception ex )
        {
            _logger.Error ( ex ,
                            "Unexpected error during Bluetooth reconnection sequence" ) ;
            return false ;
        }
        finally
        {
            IsRetrying = false ;
        }
    }

    public void Reset ( )
    {
        _logger.Debug ( "Resetting reconnect strategy state" ) ;
        CurrentAttempt = 0 ;
        IsRetrying     = false ;
    }

    /// <summary>
    ///     Calculates the delay for the current retry attempt using exponential backoff.
    /// </summary>
    /// <param name="attempt">The current attempt number (1-based).</param>
    /// <returns>Delay in milliseconds, capped at maxDelayMs.</returns>
    private int CalculateDelay ( int attempt )
    {
        // Formula: delay = initialDelay * (multiplier ^ (attempt - 1))
        // Example with defaults (1000ms initial, 2.0 multiplier):
        //   Attempt 1: 1000ms  = 1s
        //   Attempt 2: 2000ms  = 2s
        //   Attempt 3: 4000ms  = 4s
        //   Attempt 4: 8000ms  = 8s
        //   Attempt 5: 16000ms = 16s

        var delay = _initialDelayMs * Math.Pow ( _backoffMultiplier ,
                                                 attempt - 1 ) ;

        // Cap at maximum delay
        return ( int )Math.Min ( delay ,
                                 _maxDelayMs ) ;
    }
}
