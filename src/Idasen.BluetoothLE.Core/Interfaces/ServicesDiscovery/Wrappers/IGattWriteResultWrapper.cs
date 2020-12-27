using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGattWriteResultWrapper
    {
        GattCommunicationStatus Status        { get ; }
        byte?                   ProtocolError { get ; }
    }
}