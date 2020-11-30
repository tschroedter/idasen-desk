using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak
{
    public class DeskProvider   // todo tests
        : IDeskProvider
    {
        private readonly IDeskDetector      _detector ;

        public DeskProvider (
            [ NotNull ] ILogger          logger,
            [ NotNull ] IScheduler       scheduler,
            [ NotNull ] IDeskDetector    detector)
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull(scheduler,
                                  nameof(scheduler));
            Guard.ArgumentNotNull(detector,
                                  nameof(detector));

            _logger = logger ;
            _scheduler     = scheduler ;
            _detector      = detector;
        }

        /// <inheritdoc />
        public IObservable < IDesk > DeskDetected => _detector.DeskDetected ;

        /// <inheritdoc />
        public IDeskProvider Initialize ( )
        {
            _logger.Information ("Initialize...");

            _detector.Initialize (  );

            _deskDetected = _detector.DeskDetected
                                     .ObserveOn ( _scheduler )
                                     .Subscribe ( OnDeskDetected ) ;

            return this ;
        }

        /// <inheritdoc />
        public IDeskProvider StartDetecting ( )
        {
            _logger.Information("Start trying to detect desk...");

            _detector.Start (  );

            return this ;
        }

        /// <inheritdoc />
        public IDeskProvider StopDetecting()
        {
            _logger.Information("Stop trying to detect desk...");

            _detector.Stop();

            return this;
        }

        /// <inheritdoc />
        public void Dispose ( )
        {
            _deskDetected?.Dispose ( ) ;
            _detector?.Dispose ( ) ;
        }

        /// <inheritdoc />
        private void OnDeskDetected ( IDesk desk )
        {
            _logger.Debug ( "Detected desk " ) ; // todo '{desk.Name}' with address {desk.Address} " );

            _detector.Stop();
        }

        private          IDisposable _deskDetected ;
        private readonly ILogger     _logger ;
        private readonly IScheduler  _scheduler ;
    }
}