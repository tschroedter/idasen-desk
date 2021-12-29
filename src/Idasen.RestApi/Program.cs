using System ;
using Microsoft.AspNetCore.Hosting ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.Logging ;

namespace Idasen.RESTAPI
{
    /// <summary>
    /// This Program needs to be started with DAPR. The RESTAPI is using the
    /// DAPR statestore and will be called from DAPR.
    /// How to start?
    /// dapr run --app-id IdasenRESTAPI --app-port 5000 --dapr-http-port 3500 .\Idasen.RESTAPI.exe
    /// </summary>
    internal class Program
    {
        private static void Main ( )
        {
            Console.WriteLine ( "Hello World!" ) ;

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