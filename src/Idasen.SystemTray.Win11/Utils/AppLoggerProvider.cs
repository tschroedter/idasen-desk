using System.IO ;
using Idasen.Launcher ;
using Serilog ;

namespace Idasen.SystemTray.Win11.Utils ;

public static class AppLoggerProvider
{
    /// <summary>
    ///     ATTENTION: This method is a workaround for Serilog not crashing the application when the log configuration is
    ///     loaded from the appsettings.json file. The using the file will crash the application without any hint why after
    ///     a few seconds starting it. The workaround is to configure the logger programmatically for now.
    ///     (related class: Idasen.Launcher.LoggerProvider)
    ///     ToDo: This needs to be fixed properly later and involves create a crash dump file.
    /// </summary>
    /// <param name="appName"></param>
    /// <returns></returns>
    public static ILogger CreateLogger ( string appName )
    {
        // PSEUDOCODE:
        // - Determine ProgramData base path via Environment.SpecialFolder.CommonApplicationData.
        // - Create vendor/app-specific logs directory: ProgramData\Idasen\<appName>\logs.
        // - Ensure the directory exists.
        // - Build full log file path with Constants.LogFilename.
        // - Use this path for Serilog file sink.

        var programDataBase = Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ;
        var logsDirectory = Path.Combine ( programDataBase ,
                                           "Idasen" ,
                                           appName ,
                                           "logs" ) ;
        Directory.CreateDirectory ( logsDirectory ) ;

        var logFilePath = Path.Combine ( logsDirectory ,
                                         Constants.LogFilename ) ;

        return new LoggerConfiguration ( )
              .Enrich.WithProperty ( "Application" ,
                                     appName )
              .Enrich.FromLogContext ( )
              .Enrich.WithThreadId ( )
              .Enrich.WithProcessId ( )
              .Enrich.WithProcessName ( )
              .Enrich.WithMachineName ( )
              .Enrich.WithEnvironmentUserName ( )
              .Enrich.WithCaller ( ) // requires package: Serilog.Enrichers.WithCaller
              .WriteTo.Async ( w => w.File ( logFilePath ,
                                             outputTemplate :
                                             "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [ProcessId:{ProcessId}] [ThreadId:{ThreadId}] [Class:{SourceContext}] [Method:{CallerMemberName}] {Message}{NewLine}{Exception}" ,
                                             rollingInterval : RollingInterval.Day ,
                                             retainedFileCountLimit : 7 ,
                                             fileSizeLimitBytes : 1000000 ,
                                             rollOnFileSizeLimit : true ,
                                             shared : false ,
                                             hooks : new LoggingFileHooks ( ) ) )
              .CreateLogger ( ) ;
    }
}