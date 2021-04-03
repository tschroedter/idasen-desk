using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Threading ;
using System.Threading.Tasks ;
using Autofac.Extras.DynamicProxy ;
using Idasen.Aop.Aspects ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    [ Intercept ( typeof ( LogAspect ) ) ]
    public class DeskProvider
        : IDeskProvider
    {
        public DeskProvider (
            [ NotNull ] ILogger       logger ,
            [ NotNull ] ITaskRunner   taskRunner ,
            [ NotNull ] IScheduler    scheduler ,
            [ NotNull ] IDeskDetector detector ,
            [ NotNull ] IErrorManager errorManager )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( taskRunner ,
                                    nameof ( taskRunner ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( detector ,
                                    nameof ( detector ) ) ;
            Guard.ArgumentNotNull ( errorManager ,
                                    nameof ( errorManager ) ) ;

            _logger       = logger ;
            _taskRunner   = taskRunner ;
            _scheduler    = scheduler ;
            _detector     = detector ;
            _errorManager = errorManager ;
        }

        /// <inheritdoc />
        public async Task < (bool , IDesk) > TryGetDesk ( CancellationToken token )
        {
            Desk?.Dispose ( ) ;
            Desk = null ;

            try
            {
                _detector.Start ( ) ;

                await _taskRunner.Run ( ( ) => DoTryGetDesk ( token ) ,
                                        token ) ;

                if ( token.IsCancellationRequested )
                    return ( false , null ) ;

                return Desk == null
                           ? ( false , null )
                           : ( true , Desk ) ;
            }
            catch ( Exception e )
            {
                if ( e.IsBluetoothDisabledException ( ) )
                    e.LogBluetoothStatusException ( _logger ) ;
                else
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

            try
            {
                _detector.Start ( ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed Start Detecting" ) ;

                _errorManager.PublishForMessage ( "Failed Start Detecting" ) ;
            }

            return this ;
        }

        /// <inheritdoc />
        public IDeskProvider StopDetecting ( )
        {
            _logger.Information ( "Stop trying to detect desk..." ) ;

            try
            {
                _detector.Stop ( ) ;
            }
            catch ( Exception e )
            {
                _logger.Error ( e ,
                                "Failed Stop Detecting" ) ;

                _errorManager.PublishForMessage ( "Failed Stop Detecting" ) ;
            }

            return this ;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            Desk?.Dispose ( ) ; // todo test
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
            _logger.Information ( $"Detected desk {desk.Name} with " +
                                  $"Bluetooth address {desk.BluetoothAddress}" ) ;

            _detector.Stop ( ) ;

            Desk = desk ;

            DeskDetectedEvent.Set ( ) ;
        }

        private readonly IDeskDetector _detector ;
        private readonly IErrorManager _errorManager ;
        private readonly ILogger       _logger ;
        private readonly IScheduler    _scheduler ;
        private readonly ITaskRunner   _taskRunner ;

        internal readonly AutoResetEvent DeskDetectedEvent = new AutoResetEvent ( false ) ;

        private IDisposable _deskDetected ;
    }
}