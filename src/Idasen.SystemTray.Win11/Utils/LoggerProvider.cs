using Serilog ;

namespace Idasen.Launcher ;

public static class LoggerProvider
{
    public static ILogger CreateLogger ( string appName , string fileName )
    {
        return new LoggerConfiguration ( )
              .Enrich.WithProperty ( "Application" ,
                                     appName )
              .WriteTo.Async ( w => w.File ( fileName ,
                                             rollingInterval : RollingInterval.Day ,
                                             retainedFileCountLimit : 7 ,
                                             shared : true ) )
              .CreateLogger ( ) ;
    }
}
