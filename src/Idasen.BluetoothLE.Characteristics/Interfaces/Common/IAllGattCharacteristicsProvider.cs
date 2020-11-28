using System ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Common
{
    public interface IAllGattCharacteristicsProvider
    {
        [ UsedImplicitly ]
        bool TryGetDescription ( Guid       uuid ,
                                 out string description ) ;

        [ UsedImplicitly ]
        bool TryGetUuid ( string   description ,
                          out Guid uuid ) ;
    }
}