using Serilog ;
using Serilog.Configuration ;

namespace Idasen.Launcher
{
    public static class LoggerCallerEnrichmentConfiguration
    {
        public static LoggerConfiguration WithCaller ( this LoggerEnrichmentConfiguration enrichmentConfiguration )
        {
            return enrichmentConfiguration.With < CallerEnricher > ( ) ;
        }
    }
}