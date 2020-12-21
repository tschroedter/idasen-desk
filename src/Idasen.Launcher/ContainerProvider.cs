using System ;
using System.Collections.Generic ;
using System.IO ;
using Autofac ;
using Autofac.Core ;
using AutofacSerilogIntegration ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak ;
using Serilog ;
using Serilog.Events ;

namespace Idasen.Launcher
{
    public static class ContainerProvider
    {
        public static IContainer Create ( string appName ,
                                          string appLogFileName,
                                          IEnumerable <IModule> otherModules = null)
        {
            var logFolder = CreateFullPathSettingsFolderName ( appName ) ;
            var logFile   = CreateFullPathSettingsFileName(appName, appLogFileName);

            if ( ! Directory.Exists ( logFolder ) )
                Directory.CreateDirectory ( logFolder ) ;

            const string template =
                // "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message}{NewLine}{Exception}" ;
                "[{Timestamp:HH:mm:ss.ffff} {Level:u3}] {Message} (at {Caller}){NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration ( )
                        .MinimumLevel
                        .Debug (  )
                        .Enrich
                        .WithCaller ( )
                        .WriteTo
                        .ColoredConsole ( LogEventLevel.Debug ,
                                          template )
                        .WriteTo
                        .File(logFile,
                              LogEventLevel.Debug,
                              template)
                        .CreateLogger ( ) ;

            var builder = new ContainerBuilder ( ) ;

            builder.RegisterLogger ( ) ;
            builder.RegisterModule < BluetoothLECoreModule > ( ) ;
            builder.RegisterModule < BluetoothLELinakModule > ( ) ;

            if (otherModules != null)
                foreach ( var otherModule in otherModules )
                {
                    builder.RegisterModule ( otherModule ) ;
                }

            return builder.Build ( ) ;
        }

#pragma warning disable SecurityIntelliSenseCS // MS Security rules violation
        public static string CreateFullPathSettingsFileName ( string appName ,
                                                              string fileName )
        {
            var fullPath = Path.Combine ( CreateFullPathSettingsFolderName ( appName ) ,
                                          fileName ) ;
            return fullPath ;
        }

        private static string CreateFullPathSettingsFolderName ( string appName )
        {
            var appData = Environment.GetFolderPath ( Environment.SpecialFolder.CommonApplicationData) ;
            var folderName = Path.Combine ( appData ,
                                            appName ) ;

            return folderName ;
        }
#pragma warning restore SecurityIntelliSenseCS // MS Security rules violation
    }
}