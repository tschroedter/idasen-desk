using System ;

namespace Idasen.ConsoleApp
{
    internal sealed class Program
    {
        /// <summary>
        ///     Test Application
        /// </summary>
        private static void Main ( )
        {
            var demo = new Demo ( ) ;

            demo.Initialize ( )
                .Detect ( ) ;

            Console.ReadLine ( ) ;
        }
    }
}