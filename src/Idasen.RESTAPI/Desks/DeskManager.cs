using System ;
using System.Reactive.Concurrency ;
using System.Threading ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Idasen.RESTAPI.Interfaces ;
using JetBrains.Annotations ;
using Polly ;
using Serilog ;

namespace Idasen.RESTAPI.Desks
{
    internal class DeskManager
        : IDeskManager
    {
        public DeskManager ( [ NotNull ] ILogger       logger ,
                             [ NotNull ] IScheduler    scheduler ,
                             [ NotNull ] IDeskProvider provider )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( provider ,
                                    nameof ( provider ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;

            _logger    = logger ;
            _scheduler = scheduler ;
            _provider  = provider ;
        }

        public async Task < bool > Initialise ( )
        {
            void HandleException ( TimeSpan sleepDuration )
            {
                Console.WriteLine ( $"Transient error. Retrying in {sleepDuration}. " ) ;
            }

            //Build the policy
            var desk = await Policy.Handle < Exception > ( )
                                   .WaitAndRetryForeverAsync ( retryAttempt => TimeSpan.FromSeconds ( Math.Pow ( 2 ,
                                                                                                                 retryAttempt ) ) ,
                                                               ( exception ,
                                                                 sleepDuration ) =>
                                                               {
                                                                   HandleException ( sleepDuration ) ;
                                                               } )
                                   .ExecuteAsync ( FindDesk ) ;

            if ( desk is null )
                return false ;

            Desk = new RestDesk ( _logger ,
                                  _scheduler ,
                                  desk ) ;

            return true ;
        }

        public bool IsReady => Desk != null ;

        [ CanBeNull ]
        public IRestDesk Desk { get ; private set ; }

        private const string DefaultDeviceName              = "Desk" ;
        private const ulong  DefaultDeviceAddress           = 250635178951455u ;
        private const uint   DefaultDeviceMonitoringTimeout = 600u ;

        private async Task < IDesk > FindDesk ( )
        {
            var tokenSource = new CancellationTokenSource ( TimeSpan.FromSeconds ( 60 ) ) ;
            var token       = tokenSource.Token ;

            _provider.Initialize ( DefaultDeviceName ,
                                   DefaultDeviceAddress ,
                                   DefaultDeviceMonitoringTimeout ) ;

            var (isSuccess , desk) = await _provider.TryGetDesk ( token ) ;

            if ( isSuccess )
            {
                _logger.Information ( $"Found Idasen desk: {DefaultDeviceName}" ) ;
                return desk ;
            }

            _logger.Information ( $"Failed to find desk: {DefaultDeviceName}" ) ;

            return null ;
        }

        private readonly ILogger       _logger ;
        private readonly IDeskProvider _provider ;
        private readonly IScheduler    _scheduler ;
    }
}