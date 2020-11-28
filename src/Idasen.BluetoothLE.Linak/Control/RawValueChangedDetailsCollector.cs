using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using System.Reactive.Linq ;
using Idasen.BluetoothLE.Core ;
using Idasen.BluetoothLE.Linak.Interfaces ;
using JetBrains.Annotations ;
using Serilog ;

namespace Idasen.BluetoothLE.Linak.Control
{
    public class RawValueChangedDetailsCollector
        : IRawValueChangedDetailsCollector // todo maybe this class is not used
    {
        public RawValueChangedDetailsCollector ( [ NotNull ] ILogger    logger ,
                                                 [ NotNull ] IScheduler scheduler ,
                                                 [ NotNull ] IDesk      desk )
        {
            Guard.ArgumentNotNull ( logger ,
                                    nameof ( logger ) ) ;
            Guard.ArgumentNotNull ( scheduler ,
                                    nameof ( scheduler ) ) ;
            Guard.ArgumentNotNull ( desk ,
                                    nameof ( desk ) ) ;

            _logger    = logger ;
            _scheduler = scheduler ;
            _desk      = desk ;
        }

        public IRawValueChangedDetailsCollector Initialize ( )
        {
            _disposableHeight?.Dispose ( ) ;

            _disposableHeight = _desk.HeightAndSpeedChanged
                                     .ObserveOn ( _scheduler )
                                     .Subscribe ( OnHeightAndSpeedChanged ) ;

            return this ;
        }

        public IEnumerable < HeightSpeedDetails > Details => _details ;

        public void Dispose ( )
        {
            _disposableHeight?.Dispose ( ) ; // todo testing
        }

        internal const int MaxItems = 100 ;

        private void OnHeightAndSpeedChanged ( HeightSpeedDetails details )
        {
            _logger.Debug ( $"{details}" ) ;

            if ( _details.Count >= MaxItems )
                _details.RemoveAt ( 0 ) ;

            _details.Add ( details ) ;
        }

        public override string ToString ( )
        {
            return string.Join ( '|' ,
                                 Details ) ;
        }

        [ NotNull ] private readonly IDesk _desk ;

        private readonly             IList < HeightSpeedDetails > _details = new List < HeightSpeedDetails > ( ) ;
        private readonly             ILogger                      _logger ;
        [ NotNull ] private readonly IScheduler                   _scheduler ;
        private                      IDisposable                  _disposableHeight ;
    }
}