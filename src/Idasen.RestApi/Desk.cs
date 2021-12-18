using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Idasen.BluetoothLE.Linak.Interfaces;
using Idasen.Launcher;
using JetBrains.Annotations ;
using Serilog;

namespace Idasen.RestApi
{
    internal class Desk
    {
        private const string DefaultDeviceName              = "Desk";
        private const ulong  DefaultDeviceAddress           = 250635178951455u;
        private const uint   DefaultDeviceMonitoringTimeout = 600u;

        [ CanBeNull ] private IDesk _desk;

        public async Task Initialise ( )
        {
            _desk = await FindDesk ( ).ConfigureAwait ( false ) ;
        }

        private async Task<IDesk> FindDesk ( )
        {
            var tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
            var token       = tokenSource.Token ;

            var container = ContainerProvider.Create ( "Idasen.ConsoleApp" ,
                                                       "Idasen.ConsoleApp.log" ) ;

            var logger   = container.Resolve < ILogger > ( ) ;
            var provider = container.Resolve < IDeskProvider > ( ) ;

            provider.Initialize ( DefaultDeviceName ,
                                  DefaultDeviceAddress ,
                                  DefaultDeviceMonitoringTimeout ) ;

            var (isSuccess , desk) = await provider.TryGetDesk ( token ) ;

            if ( isSuccess )
            {
                logger.Information ( $"Found Idasen desk: {DefaultDeviceName}" ) ;
                return desk ;
            }

            logger.Information($"Failed to find desk: {DefaultDeviceName}");

            return null ;
        }
    }
}
