using System ;
using Microsoft.AspNetCore.Hosting ;

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
                      .UseStartup < Startup > ( )
                      .Build ( ) ;

            host.Run ( ) ;
        }
    }
}