using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Windows.Storage.Streams ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    public class GattReadResultWrapperNotSupported
        : IGattReadResultWrapper
    {
        public GattCommunicationStatus Status { get ; } = GattCommunicationStatus.Unreachable ;

        public byte?   ProtocolError { get ; } = null ;
        public IBuffer Value         { get ; } = null ;
    }
}