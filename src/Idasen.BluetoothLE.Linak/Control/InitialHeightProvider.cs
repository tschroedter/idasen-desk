using System ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using System.Reactive.Subjects ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Control
{
    /// <inheritdoc />
    public class InitialHeightProvider
        : IInitialHeightProvider
    {
        public InitialHeightProvider ( [ NotNull ] ILogger              logger ,
                                       [ NotNull ] IScheduler           scheduler ,
                                       [ NotNull ] IDeskHeightAndSpeed  heightAndSpeed ,
                                       [ NotNull ] IDeskCommandExecutor executor ,
                                       [ NotNull ] ISubject < uint >    subjectFinished )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( heightAndSpeed ,
                                    nameof ( heightAndSpeed ) ) ;
            Guard.ArgumentNotNull ( executor ,
                                    nameof ( executor ) ) ;
            Guard.ArgumentNotNull ( subjectFinished ,
                                    nameof ( subjectFinished ) ) ;

            _logger          = logger ;
            _scheduler       = scheduler ;
            _heightAndSpeed  = heightAndSpeed ;
            _executor        = executor ;
            _subjectFinished = subjectFinished ;
        }

        /// <inheritdoc />
        public void Initialize ( )
        {
            _disposalHeightAndSpeed?.Dispose ( ) ;

            _disposalHeightAndSpeed = _heightAndSpeed.HeightAndSpeedChanged
                                                     .SubscribeOn ( _scheduler )
                                                     .Subscribe ( OnHeightAndSpeedChanged ) ;

            Height = _heightAndSpeed.Height ;
        }

        /// <inheritdoc />
        public async Task Start ( )
        {
            if ( _disposalHeightAndSpeed == null )
                throw new NotInitializeException ( "Initialize needs to be called first" ) ;

            if ( _heightAndSpeed.Height > 0 )
            {
                _logger.Information ( $"Current height is {_heightAndSpeed.Height}" ) ;

                _isReceivedHeightAndSpeed = true ;

                return ;
            }

            _logger.Information ( "Trying to determine current height by moving the desk" ) ;

            _isReceivedHeightAndSpeed = false ;

            await _executor.Up ( ) ;
            await _executor.Stop ( ) ;
        }

        /// <inheritdoc />
        public IObservable < uint > Finished => _subjectFinished ;

        /// <inheritdoc />
        public void Dispose ( )
        {
            _disposalHeightAndSpeed?.Dispose ( ) ;
        }

        /// <inheritdoc />
        public uint Height { get ; private set ; }

        public delegate IInitialHeightProvider Factory ( IDeskCommandExecutor executor ,
                                                         IDeskHeightAndSpeed  heightAndSpeed ) ;

        private void OnHeightAndSpeedChanged ( HeightSpeedDetails details )
        {
            Height = details.Height ;

            if ( _isReceivedHeightAndSpeed )
                return ;

            if ( details.Height <= 0 )
            {
                _logger.Information ( "Received invalid "             +
                                      $"height {details.Height} and " +
                                      $"speed {details.Speed} ..." ) ;

                return ;
            }

            _subjectFinished.OnNext ( Height ) ;
            _isReceivedHeightAndSpeed = true ;

            _logger.Information ( "Received valid " +
                                  $"height {details.Height}." ) ;
        }

        private readonly IDeskCommandExecutor _executor ;
        private readonly IDeskHeightAndSpeed  _heightAndSpeed ;

        private readonly ILogger           _logger ;
        private readonly IScheduler        _scheduler ;
        private readonly ISubject < uint > _subjectFinished ;

        private IDisposable _disposalHeightAndSpeed ;
        private bool        _isReceivedHeightAndSpeed ;
    }
}