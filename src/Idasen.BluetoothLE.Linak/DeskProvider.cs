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
    public class DeskProvider
        : IDeskProvider
    {
        public DeskProvider (
            [ NotNull ] ILogger       logger ,
            [ NotNull ] ITaskRunner   taskRunner ,
            [ NotNull ] IScheduler    scheduler ,
            [ NotNull ] IDeskDetector detector )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( taskRunner ,
                                    nameof ( taskRunner ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( detector ,
                                    nameof ( detector ) ) ;

            _logger     = logger ;
            _taskRunner = taskRunner ;
            _scheduler  = scheduler ;
            _detector   = detector ;
        }

        /// <inheritdoc />
        public async Task < (bool , IDesk) > TryGetDesk ( CancellationToken token )
        {
            if ( Desk != null )
                return ( true , Desk ) ;

            try
            {
                _detector.Start ( ) ;

                await _taskRunner.Run ( ( ) => DoTryGetDesk ( token ) ,
                                        token ) ;

                return token.IsCancellationRequested
                           ? ( false , null )
                           : ( true , Desk ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed to detect desk" ) ;

                return ( false , null ) ;
            }
            finally
            {
                _detector.Stop ( ) ;
                DeskDetectedEvent.Reset ( ) ;
            }
        }

        /// <inheritdoc />
        public IObservable < IDesk > DeskDetected => _detector.DeskDetected ;

        /// <inheritdoc />
        public IDeskProvider Initialize ( string deviceName ,
                                          ulong  deviceAddress ,
                                          uint   deviceTimeout )
        {
            Guard.ArgumentNotNull ( deviceName ,
                                    nameof ( deviceName ) ) ;

            _logger.Information ( "Initialize..." ) ;

            _detector.Initialize ( deviceName ,
                                   deviceAddress ,
                                   deviceTimeout ) ;

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

        /// <inheritdoc />
        public IDesk Desk { get ; private set ; }

        internal void DoTryGetDesk ( CancellationToken token )
        {
            while ( Desk == null &&
                    ! token.IsCancellationRequested )
            {
                _logger.Information ( "Trying to find desk..." ) ;

                DeskDetectedEvent.WaitOne ( TimeSpan.FromSeconds ( 1 ) ) ;
            }
        }

        internal void OnDeskDetected ( IDesk desk )
        {
            _logger.Debug ( $"Detected desk {desk.Name} with " +
                            $"Bluetooth address {desk.BluetoothAddress}" ) ;

            _detector.Stop ( ) ;

            Desk = desk ;

            DeskDetectedEvent.Set ( ) ;
        }

        private readonly IDeskDetector _detector ;
        private readonly ILogger       _logger ;
        private readonly IScheduler    _scheduler ;
        private readonly ITaskRunner   _taskRunner ;

        internal readonly AutoResetEvent DeskDetectedEvent = new AutoResetEvent ( false ) ;

        private IDisposable _deskDetected ;
    }
}