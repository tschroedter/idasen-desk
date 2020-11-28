using System ;
using System.Collections.Generic ;
using System.Collections.Immutable ;
using Windows.Devices.Bluetooth ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Characteristics.Common ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Characteristics.Characteristics.Unknowns
{
    public class Device
        : IDevice
    {
        public void Dispose ( )
        {
        }

        public IObservable < BluetoothConnectionStatus > ConnectionStatusChanged =>
            throw new NotInitializeException ( Message ) ;

        public GattCommunicationStatus   GattCommunicationStatus { get ; } = GattCommunicationStatus.Unreachable ;
        public string                    Name { get ; } = UnknownDeviceName ;
        public string                    DeviceId { get ; } = UnknownDeviceId ; // todo maybe rename to just Id
        public bool                      IsPaired { get ; } = false ;
        public BluetoothConnectionStatus ConnectionStatus { get ; } = BluetoothConnectionStatus.Disconnected ;

        public IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > GattServices
        {
            get ;
        } = new Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > ( )
           .ToImmutableDictionary ( ) ;

        public IObservable < GattCommunicationStatus > GattServicesRefreshed =>
            throw new NotInitializeException ( Message ) ;

        public void Connect ( )
        {
            // do nothing
        }

        internal const string UnknownDeviceName = "Unknown Device" ;
        internal const string UnknownDeviceId   = "Unknown Device Id" ;
        internal const string Message           = "Can't use a anknown Instance" ;
    }
}