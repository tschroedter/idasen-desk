using System ;
using System.IO ;
using Idasen.BluetoothLE.Core ;
using JetBrains.Annotations ;
using Serilog ;
using Serilog.Events ;
using Serilog.Sinks.SystemConsole.Themes ;

namespace Idasen.Launcher
{
    public static class LoggerProvider
    {
        private const string LogTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.ffff} " +
                                           "{Level:u3}] {Message} "                 +
                                           "(at {Caller}){NewLine}{Exception}" ;

        private static ILogger Logger ;

        public static string LogFile { get ; private set ; }

        public static string LogFolder { get ; private set ; }

        public static ILogger CreateLogger (
            [ NotNull ] string appName ,
            [ NotNull ] string appLogFileName )
        {
            Guard.ArgumentNotNull ( appName ,
                                    nameof ( appName ) ) ;
            Guard.ArgumentNotNull ( appLogFileName ,
                                    nameof ( appLogFileName ) ) ;

            if ( Logger != null )
            {
                Logger.Debug ( $"Using existing logger for '{appName}' in folder {appLogFileName}" ) ;

                return Logger ;
            }

            Logger = DoCreateLogger ( appName ,
                                      appLogFileName ) ;

            return Logger ;
        }

        private static ILogger DoCreateLogger (
            string appName ,
            string appLogFileName )
        {
            LogFolder = CreateFullPathApplicationFolderName ( appName ) ;
            LogFile = CreateFullPathLogFileName ( appName ,
                                                  appLogFileName ) ;

            if ( ! Directory.Exists ( LogFolder ) )
                Directory.CreateDirectory ( LogFolder ) ;

            var logger = new LoggerConfiguration ( )
                        .MinimumLevel
                        .Debug ( )
                        .Enrich
                        .WithCaller ( )
                        .WriteTo.Console ( LogEventLevel.Debug ,
                                           LogTemplate ,
                                           theme : AnsiConsoleTheme.Code )
                        .WriteTo
                        .File ( LogFile ,
                                LogEventLevel.Debug ,
                                LogTemplate )
                        .CreateLogger ( ) ;

            var message = $"Created logger for '{appName}' in folder '{LogFile}'" ;

            Logger.Information ( message ) ;

            return logger ;
        }

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        public static string CreateFullPathLogFileName ( string appName ,
                                                         string fileName )
        {
            var fullPath = Path.Combine ( CreateFullPathApplicationFolderName ( appName ) ,
                                          fileName ) ;
            return fullPath ;
        }

        public static string CreateFullPathApplicationFolderName ( string appName )
        {
            var appData = Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData ) ;
            var folderName = Path.Combine ( appData ,
                                            appName ) ;

            return folderName ;
        }
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
    }
}