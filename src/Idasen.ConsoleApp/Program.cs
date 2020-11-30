using System;
using System.Reactive.Concurrency ;
using System.Reactive.Linq;
using Autofac;
using Idasen.BluetoothLE.Linak.Interfaces;
using static System.Console;

namespace Idasen.ConsoleApp
{
    internal sealed class Program
    {
        private static IDesk Desk ;

        /// <summary>
        ///     Test Application
        /// </summary>
        private static void Main ( )
        {
            var container = ContainerProvider.Create( ) ;

            var scheduler = container.Resolve < IScheduler > ( ) ;
            var provider  = container.Resolve < IDeskProvider > ( ) ;

            provider.Initialize ( );

            using var _ = provider.DeskDetected
                                  .ObserveOn(scheduler)
                                  .Subscribe(OnDeskDetected) ;

            provider.StartDetecting (  );

            ReadLine ( ) ;

            provider.Initialize()
                    .StopDetecting();
        }

        private static void OnDeskDetected ( IDesk desk )
        {
            Desk = desk ;

            Desk.MoveTo ( 7200u );
        }
    }
}