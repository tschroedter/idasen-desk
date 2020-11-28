using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Characteristics.Interfaces.Characteristics.Customs
{
    public interface IGattCharacteristicsProviderFactory
    {
        IGattCharacteristicProvider Create ( [ NotNull ] IGattCharacteristicsResultWrapper wrapper ) ;
    }
}