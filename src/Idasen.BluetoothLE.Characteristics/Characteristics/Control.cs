using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using System.Threading.Tasks ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class Control
        : CharacteristicBase ,
          IControl
    {
        public Control (
            ILogger                              logger ,
            IScheduler                           scheduler ,
            IDevice                              device ,
            IGattCharacteristicsProviderFactory  providerFactory ,
            IRawValueReader                      rawValueReader ,
            IRawValueWriter                      rawValueWriter ,
            ICharacteristicBaseToStringConverter toStringConverter ,
            IDescriptionToUuid                   descriptionToUuid )
            : base ( logger ,
                     scheduler ,
                     device ,
                     providerFactory ,
                     rawValueReader ,
                     rawValueWriter ,
                     toStringConverter ,
                     descriptionToUuid )
        {
        }

        public IEnumerable < byte > RawControl2 => GetValueOrEmpty ( Control2Key ) ;

        public IEnumerable < byte > RawControl3 => GetValueOrEmpty ( Control3Key ) ;

        public async Task < bool > TryWriteRawControl2 ( IEnumerable < byte > bytes )
        {
            return await TryWriteValueAsync ( Control2Key ,
                                              bytes ) ;
        }

        public delegate IControl Factory ( IDevice device ) ;

        internal const string Control2Key = "Ctrl2" ;
        internal const string Control3Key = "Ctrl3" ;

        public override Guid GattServiceUuid { get ; } = Guid.Parse ( "99FA0001-338A-1024-8A49-009C0215F78A" ) ;

        protected override T WithMapping < T > ( ) where T : class
        {
            DescriptionToUuid [ Control2Key ] = Guid.Parse ( "99fa0002-338a-1024-8a49-009c0215f78a" ) ;
            DescriptionToUuid [ Control3Key ] = Guid.Parse ( "99fa0003-338a-1024-8a49-009c0215f78a" ) ;

            return this as T ;
        }
    }
}