using Serilog ;
using Serilog.Configuration ;

namespace Idasen.ConsoleApp
{
    public static class LoggerCallerEnrichmentConfiguration
    {
        public static LoggerConfiguration WithCaller ( this LoggerEnrichmentConfiguration enrichmentConfiguration )
        {
            return enrichmentConfiguration.With < CallerEnricher > ( ) ;
        }
    }
}