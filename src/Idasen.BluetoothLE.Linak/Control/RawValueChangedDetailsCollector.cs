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
        : IRawValueChangedDetailsCollector
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

            _logger = logger ;

            _disposableHeight = desk.HeightAndSpeedChanged
                                    .ObserveOn ( scheduler )
                                    .Subscribe ( OnHeightAndSpeedChanged ) ;
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

        private readonly IList < HeightSpeedDetails > _details = new List < HeightSpeedDetails > ( ) ;
        private readonly IDisposable                  _disposableHeight ;
        private readonly ILogger                      _logger ;
    }
}