using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGattCharacteristicWrapperFactory
    {
        IGattCharacteristicWrapper Create ( GattCharacteristic characteristic ) ;
    }
}