using System ;
using System.Collections.Generic ;
using System.Reactive.Subjects ;
using System.Threading ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Linak ;
using Idasen.RESTAPI.Interfaces ;

namespace Idasen.RESTAPI.Desks
{
    internal class FakeDesk : IRestDesk
    {
        public FakeDesk ( )
        {
            Name                 = "Fake Desk" ;
            BluetoothAddress     = 1u ;
            BluetoothAddressType = "Fake Address Type" ;
            DeviceName           = "Fake Desk Device" ;
        }

        public void Dispose ( )
        {
        }

        public IObservable < IEnumerable < byte > > DeviceNameChanged     => _deviceNameChanged ;
        public IObservable < uint >                 HeightChanged         => _heightChanged ;
        public IObservable < int >                  SpeedChanged          => _speedChanged ;
        public IObservable < HeightSpeedDetails >   HeightAndSpeedChanged => _heightAndSpeedChanged ;
        public IObservable < uint >                 FinishedChanged       => _finishedChanged ;
        public IObservable < bool >                 RefreshedChanged      => _refreshedChanged ;
        public string                               Name                  { get ; }
        public ulong                                BluetoothAddress      { get ; }
        public string                               BluetoothAddressType  { get ; }
        public string                               DeviceName            { get ; }

        public void Connect ( )
        {
        }

        public void MoveTo ( uint targetHeight )
        {
            DoMoveTo ( targetHeight ) ;
        }

        public void MoveUp ( )
        {
            DoMoveUp ( ) ;
        }

        public void MoveDown ( )
        {
            DoMoveDown ( ) ;
        }

        public void MoveStop ( )
        {
            DoMoveStop ( ) ;
        }

        public Task < bool > MoveToAsync ( uint targetHeight )
        {
            return DoAction ( ( ) => DoMoveTo ( targetHeight ) ) ;
        }

        public Task < bool > MoveUpAsync ( )
        {
            return DoAction ( DoMoveUp ) ;
        }

        public Task < bool > MoveDownAsync ( )
        {
            return DoAction ( DoMoveDown ) ;
        }

        public Task < bool > MoveStopAsync ( )
        {
            return DoAction ( DoMoveStop ) ;
        }

        public uint Height { get ; private set ; } = 6000u ;

        public int Speed { get ; private set ; }

        public uint Step      { get ; set ; } = 100u ;
        public int  StepSpeed { get ; set ; } = 25 ;

        public TimeSpan DefaultStepSleep { get ; set ; } = TimeSpan.FromSeconds ( 0.5 ) ;

        public bool IsInUse { get ; private set ; }

        private void CreateNewSourceAndToken ( )
        {
            if ( _source != null )
            {
                _source.Cancel ( ) ;
                _source.Dispose ( ) ;
            }

            _source = new CancellationTokenSource ( ) ;
            _source.CancelAfter ( _defaultTimeout ) ;
            _token = _source.Token ;
        }

        private void DoMoveTo ( uint targetHeight )
        {
            if ( CheckIfIsInUse ( ) )
                return ;

            var steps      = Math.Abs ( ( ( int )targetHeight - ( int )Height ) / ( int )Step ) ;
            var isMoveDown = targetHeight <= Height ;

            for ( var i = 1 ; i < steps ; i ++ )
            {
                if ( isMoveDown )
                    DoMoveDownOneStep ( ) ;
                else
                    DoMoveUpOneStep ( ) ;
            }

            Height = targetHeight ;
            Speed  = 0 ;

            FinishedMove ( ) ;
        }

        private bool CheckIfIsInUse ( )
        {
            lock ( _padlock )
            {
                if ( IsInUse )
                    return false ;

                IsInUse = true ;
            }

            return true ;
        }

        private void DoMoveUp ( )
        {
            if ( CheckIfIsInUse ( ) )
                return ;

            DoMoveUpOneStep ( ) ;
        }

        private void DoMoveUpOneStep ( )
        {
            Height = ( uint )( ( int )Height + ( int )Step ) ;
            Speed  = StepSpeed ;

            PublishHeightAndSpeed ( ) ;

            Thread.Sleep ( DefaultStepSleep ) ;

            FinishedMove ( ) ;
        }

        private void DoMoveDown ( )
        {
            if ( CheckIfIsInUse ( ) )
                return ;

            DoMoveDownOneStep ( ) ;
        }

        private void DoMoveDownOneStep ( )
        {
            Height = ( uint )( ( int )Height - ( int )Step ) ;
            Speed  = - StepSpeed ;

            PublishHeightAndSpeed ( ) ;

            Thread.Sleep ( DefaultStepSleep ) ;

            FinishedMove ( ) ;
        }

        private void DoMoveStop ( )
        {
            Thread.Sleep ( DefaultStepSleep ) ;

            FinishedMove ( ) ;
        }

        private async Task < bool > DoAction ( Action action )
        {
            lock ( _padlock )
            {
                if ( IsInUse )
                    return false ;

                IsInUse = true ;
            }

            CreateNewSourceAndToken ( ) ;

            await Task.Run ( action ,
                             _token ) ;

            lock ( _padlock )
            {
                IsInUse = false ;
            }

            return true ;
        }

        private void PublishHeightAndSpeed ( )
        {
            _heightChanged.OnNext ( Height ) ;
            _speedChanged.OnNext ( Speed ) ;

            var details = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                   Height ,
                                                   Speed ) ;

            _heightAndSpeedChanged.OnNext ( details ) ;
        }

        private void FinishedMove ( )
        {
            Speed = 0 ;

            _heightChanged.OnNext ( Height ) ;
            _speedChanged.OnNext ( Speed ) ;

            var detailsZero = new HeightSpeedDetails ( DateTimeOffset.Now ,
                                                       Height ,
                                                       0 ) ;

            _heightAndSpeedChanged.OnNext ( detailsZero ) ;
        }

        private readonly TimeSpan _defaultTimeout = TimeSpan.FromMinutes ( 1 ) ;

        private readonly Subject < IEnumerable < byte > >
            _deviceNameChanged = new Subject < IEnumerable < byte > > ( ) ;

        private readonly Subject < uint > _finishedChanged = new Subject < uint > ( ) ;

        private readonly Subject < HeightSpeedDetails >
            _heightAndSpeedChanged = new Subject < HeightSpeedDetails > ( ) ;

        private readonly Subject < uint > _heightChanged    = new Subject < uint > ( ) ;
        private readonly object           _padlock          = new object ( ) ;
        private readonly Subject < bool > _refreshedChanged = new Subject < bool > ( ) ;

        private readonly Subject < int >         _speedChanged = new Subject < int > ( ) ;
        private          CancellationTokenSource _source ;
        private          CancellationToken       _token ;
    }
}