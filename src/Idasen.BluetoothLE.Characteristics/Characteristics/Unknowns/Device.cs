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
        public string                    Name                    { get ; } = UnknownName ;
        public string                    Id                      { get ; } = UnknownId ;
        public bool                      IsPaired                { get ; } = false ;
        public BluetoothConnectionStatus ConnectionStatus        { get ; } = BluetoothConnectionStatus.Disconnected ;

        public IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > GattServices
        {
            get ;
        } = new Dictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > ( )
           .ToImmutableDictionary ( ) ;

        public IObservable < GattCommunicationStatus > GattServicesRefreshed =>
            throw new NotInitializeException ( Message ) ;

        public ulong BluetoothAddress => UnknownBluetoothAddress ;

        public string BluetoothAddressType => UnknownBluetoothAddressType ;

        public void Connect ( )
        {
            // do nothing
        }

        internal const ulong UnknownBluetoothAddress = 0u ;

        internal const string UnknownBluetoothAddressType = "Unknown Address Type" ;
        internal const string UnknownName                 = "Unknown Device" ;
        internal const string UnknownId                   = "Unknown Device Id" ;
        internal const string Message                     = "Can't use a anknown Instance" ;
    }
}