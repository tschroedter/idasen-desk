using System ;
using System.IO ;
using Microsoft.AspNetCore.Hosting ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.Logging ;
using static System.Console ;

namespace Idasen.RESTAPI
{
    /// <summary>
    ///     This Program needs to be started with DAPR. The RESTAPI is using the
    ///     DAPR statestore and will be called from DAPR.
    ///     How to start?
    ///     dapr run --app-id IdasenRESTAPI --app-port 5000 --dapr-http-port 3500
    ///     C:\Development\GitHub\idasen-desk\src\Idasen.RESTAPI\bin\Debug\netcoreapp3.1\Idasen.RESTAPI.exe
    /// </summary>
    internal class Program
    {
        private static void Main ( )
        {
            PrintInformation ( ) ;

            var host = new WebHostBuilder ( )
                      .UseKestrel ( )
                      .UseUrls ( "http://*:5000" )
                      .ConfigureLogging ( logging =>
                                          {
                                              logging.ClearProviders ( ) ;
                                              logging.AddConsole ( ) ;
                                              logging.AddDebug ( ) ;
                                          } )
                      .UseStartup < Startup > ( )
                      .ConfigureAppConfiguration ( AddAppSettingsJson ( ) )
                      .Build ( ) ;

            host.Run ( ) ;
        }

        private static void PrintInformation ( )
        {
            WriteLine ( "Idasen.RESTAPI uses DAPR" ) ;
            WriteLine ( "Run it by using the following command line:" ) ;
            WriteLine ( @"dapr run --app-id IdasenRESTAPI --app-port 5000 "           +
                        $@"--dapr-http-port 3500 {Directory.GetCurrentDirectory ( )}" +
                        Path.DirectorySeparatorChar                                   +
                        @"Idasen.RESTAPI.exe" ) ;
            WriteLine ( ) ;
        }

        private static Action < WebHostBuilderContext , IConfigurationBuilder > AddAppSettingsJson ( )
        {
            return ( context ,
                     builder ) =>
                   {
                       builder.AddJsonFile ( "appsettings.json" ,
                                             true ) ;
                   } ;
        }
    }
}