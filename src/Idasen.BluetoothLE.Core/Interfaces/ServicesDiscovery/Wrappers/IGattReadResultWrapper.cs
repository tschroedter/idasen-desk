using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IGattReadResultWrapper
    {
        GattCommunicationStatus Status        { get ; }
        byte?                   ProtocolError { get ; }
        IBuffer                 Value         { get ; }
    }
}