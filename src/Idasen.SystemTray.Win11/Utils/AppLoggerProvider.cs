using Idasen.Launcher ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public static class AppLoggerProvider
{
    public static ILogger CreateLogger ( string appName , string fileName )
    {
        return new LoggerConfiguration ( )
              .Enrich.WithProperty ( "Application" ,
                                     appName )
              .Enrich.WithThreadId ( )
              .Enrich.WithProcessId ( )
              .Enrich.WithProcessName ( )
              .Enrich.WithMachineName ( )
              .Enrich.WithEnvironmentUserName ( )
              .WriteTo.Async ( w => w.File ( fileName ,
                                             rollingInterval : RollingInterval.Day ,
                                             retainedFileCountLimit : 7 ,
                                             shared : false,
                                             hooks : new LoggingFileHooks (  )) )
              .CreateLogger ( ) ;
    }
}