using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGattCharacteristicWrapperFactory
    {
        IGattCharacteristicWrapper Create ( [ NotNull ] GattCharacteristic characteristic ) ;
    }
}