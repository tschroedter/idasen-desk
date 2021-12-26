using System ;
using Microsoft.AspNetCore.Hosting ;
using Microsoft.Extensions.Configuration ;
using Microsoft.Extensions.Logging ;

namespace Idasen.RESTAPI
{
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
                                          })
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
                                             optional : true ) ;
                   } ;
        }
    }
}