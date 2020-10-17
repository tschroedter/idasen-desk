using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.ServicesDiscovery.Wrappers
{
    public class GattWriteResultNotSupported
        : IGattWriteResult
    {
        public GattCommunicationStatus Status { get ; } = GattCommunicationStatus.Unreachable ;

        public byte? ProtocolError { get ; } = null ;
    }
}