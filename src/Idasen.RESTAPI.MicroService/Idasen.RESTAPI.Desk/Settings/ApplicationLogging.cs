namespace Idasen.RESTAPI.Desk.Settings ;

/// <summary>
///     Shared logger
/// </summary>
internal static class ApplicationLogging
{
    internal static ILoggerFactory LoggerFactory { get ; set ; } = new LoggerFactory ( ) ;

    internal static ILogger CreateLogger < T > ( )
    {
        return LoggerFactory.CreateLogger < T > ( ) ;
    }

    internal static ILogger CreateLogger ( string categoryName )
    {
        return LoggerFactory.CreateLogger ( categoryName ) ;
    }
}