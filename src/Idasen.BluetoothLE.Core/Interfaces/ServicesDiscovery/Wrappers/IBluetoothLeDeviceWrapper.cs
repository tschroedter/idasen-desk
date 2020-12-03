using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    public interface IBluetoothLeDeviceWrapper
        : IDisposable
    {
        /// <summary>
        ///     Gets the device Id.
        /// </summary>
        string DeviceId { get ; }

        /// <summary>
        ///     Gets the name of the Bluetooth LE device.
        /// </summary>
        string Name { get ; }

        /// <summary>
        ///     Gets the connection status of the device.
        /// </summary>
        BluetoothConnectionStatus ConnectionStatus { get ; }

        /// <summary>
        ///     ???
        /// </summary>
        bool IsPaired { get ; }

        /// <summary>
        ///     Defines a provider for push-based notification about the
        ///     connection status..
        /// </summary>
        IObservable < BluetoothConnectionStatus > ConnectionStatusChanged { get ; }

        /// <summary>
        ///     Gets the read-only list of GATT services supported by the device.
        /// </summary>
        IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > GattServices { get ; }

        /// <summary>
        ///     Defines a provider for push-based notification about Gatt services
        ///     being refreshed.
        /// </summary>
        IObservable < GattCommunicationStatus > GattServicesRefreshed { get ; }

        /// <summary>
        ///     Gets the Gatt communication connection status of the device.
        /// </summary>
        GattCommunicationStatus GattCommunicationStatus { get ; }

        /// <summary>
        ///     The device's address
        /// </summary>
        ulong BluetoothAddress { get ; }

        /// <summary>
        ///     The address type.
        /// </summary>
        string BluetoothAddressType { get ; }

        /// <summary>
        ///     Connects to the device if the device is disconnected.
        /// </summary>
        void Connect ( ) ;

        /// <summary>
        ///     Gets the Gatt services for the device.
        /// </summary>
        /// <returns></returns>
        Task < IGattDeviceServicesResultWrapper > GetGattServicesAsync ( ) ;
    }
}