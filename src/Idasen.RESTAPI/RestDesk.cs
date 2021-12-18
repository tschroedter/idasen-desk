using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Linak ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using Serilog ;

namespace Idasen.RESTAPI
{
    public class RestDesk : IRestDesk
    {
        public RestDesk ( ILogger logger ,
                          IDesk   desk )
        {
            _logger = logger ;
            _desk   = desk ;

            Height = 0u ;
            Speed  = 0 ;
        }

        public void Dispose ( )
        {
            _desk.Dispose ( ) ;
        }

        public IObservable < IEnumerable < byte > > DeviceNameChanged => _desk.DeviceNameChanged ;

        public IObservable < uint > HeightChanged => _desk.HeightChanged ;

        public IObservable < int > SpeedChanged => _desk.SpeedChanged ;

        public IObservable < HeightSpeedDetails > HeightAndSpeedChanged => _desk.HeightAndSpeedChanged ;

        public IObservable < uint > FinishedChanged => _desk.FinishedChanged ;

        public IObservable < bool > RefreshedChanged => _desk.RefreshedChanged ;

        public string Name => _desk.Name ;

        public ulong BluetoothAddress => _desk.BluetoothAddress ;

        public string BluetoothAddressType => _desk.BluetoothAddressType ;

        public string DeviceName => _desk.DeviceName ;

        public void Connect ( )
        {
            _desk.Connect ( ) ;
        }

        public void MoveTo ( uint targetHeight )
        {
            _desk.MoveTo ( targetHeight ) ;
        }

        public void MoveUp ( )
        {
            _desk.MoveUp ( ) ;
        }

        public void MoveDown ( )
        {
            _desk.MoveDown ( ) ;
        }

        public void MoveStop ( )
        {
            _desk.MoveStop ( ) ;
        }

        public uint Height { get ; }
        public int  Speed  { get ; }

        public Task < bool > MoveToAsync ( uint targetHeight )
        {
            return DoAction ( ( ) => MoveTo ( targetHeight ) ) ;
        }

        public Task < bool > MoveUpAsync ( )
        {
            return DoAction ( MoveUp ) ;
        }

        public Task < bool > MoveDownAsync ( )
        {
            return DoAction ( MoveDown ) ;
        }

        public Task < bool > MoveStopAsync ( )
        {
            return DoAction ( MoveStop ) ;
        }

        private async Task < bool > DoAction ( Action action )
        {
            try
            {
                await Task.Run ( action ) ;

                return true ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to execute action" ) ;

                return false ;
            }
        }

        private readonly IDesk _desk ;

        private readonly ILogger _logger ;
    }
}