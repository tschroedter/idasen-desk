using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Threading ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    public class DeskProvider // todo tests
        : IDeskProvider
    {
        public DeskProvider (
            [ NotNull ] ILogger       logger ,
            [ NotNull ] IScheduler    scheduler ,
            [ NotNull ] IDeskDetector detector )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( detector ,
                                    nameof ( detector ) ) ;

            _logger    = logger ;
            _scheduler = scheduler ;
            _detector  = detector ;
        }

        /// <inheritdoc />
        public async Task < (bool , IDesk) > TryGetDesk ( CancellationToken token )
        {
            if ( _desk != null )
                return ( true , _desk ) ;

            try
            {
                _detector.Start ( ) ;

                await Task.Run ( DoTryGetDesk ,
                                 token ) ;

                return token.IsCancellationRequested
                           ? ( false , null )
                           : ( true , _desk ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to detect desk" ) ;

                return ( false , null ) ;
            }
            finally
            {
                _detector.Start ( ) ;
                _deskDetectedEvent.Reset ( ) ;
            }
        }

        /// <inheritdoc />
        public IObservable < IDesk > DeskDetected => _detector.DeskDetected ;

        /// <inheritdoc />
        public IDeskProvider Initialize ( )
        {
            _logger.Information ( "Initialize..." ) ;

            _detector.Initialize ( ) ;

            _deskDetected = _detector.DeskDetected
                                     .ObserveOn ( _scheduler )
                                     .Subscribe ( OnDeskDetected ) ;

            return this ;
        }

        /// <inheritdoc />
        public IDeskProvider StartDetecting ( )
        {
            _logger.Information ( "Start trying to detect desk..." ) ;

            _detector.Start ( ) ;

            return this ;
        }

        /// <inheritdoc />
        public IDeskProvider StopDetecting ( )
        {
            _logger.Information ( "Stop trying to detect desk..." ) ;

            _detector.Stop ( ) ;

            return this ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _deskDetected?.Dispose ( ) ;
            _detector?.Dispose ( ) ;
        }

        private void DoTryGetDesk ( )
        {
            while ( _desk == null )
            {
                _logger.Information ( "Trying to find desk..." ) ;

                _deskDetectedEvent.WaitOne ( TimeSpan.FromSeconds ( 1 ) ) ;
            }
        }

        /// <inheritdoc />
        private void OnDeskDetected ( IDesk desk )
        {
            _logger.Debug ( $"Detected desk {desk.Name} with " +
                            $"Bluetooth address {desk.BluetoothAddress}" ) ;

            _detector.Stop ( ) ;

            _desk = desk ;

            _deskDetectedEvent.Set ( ) ;
        }

        private readonly      AutoResetEvent _deskDetectedEvent = new AutoResetEvent ( false ) ;
        private readonly      IDeskDetector  _detector ;
        private readonly      ILogger        _logger ;
        private readonly      IScheduler     _scheduler ;
        [ CanBeNull ] private IDesk          _desk ;

        private IDisposable _deskDetected ;
    }
}