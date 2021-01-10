using System ;
using System.Collections.Generic ;
using System.Reactive.Concurrency ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs ;
using Idasen.BluetoothLE.Characteristics.Interfaces.Common ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Serilog ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics
{
    public class Dpg
        : CharacteristicBase ,
          IDpg
    {
        public Dpg (
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


        public IEnumerable < byte > RawDpg => GetValueOrEmpty ( DpgKey ) ;

        public delegate IDpg Factory ( IDevice device ) ;

        public const string DpgKey = "Dpg" ;

        public override Guid GattServiceUuid { get ; } = Guid.Parse ( "99FA0010-338A-1024-8A49-009C0215F78A" ) ;

        protected override T WithMapping < T > ( ) where T : class
        {
            DescriptionToUuid [ DpgKey ] = Guid.Parse ( "99FA0011-338A-1024-8A49-009C0215F78A" ) ;

            return this as T ;
        }
    }
}