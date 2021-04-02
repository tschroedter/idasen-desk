using System ;
using System.Collections.Generic ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery
{
    public interface IGattServicesProvider
        : IDisposable
    {
        /// <summary>
        ///     Gets the read-only list of GATT services supported by the device.
        /// </summary>
        IReadOnlyDictionary < IGattDeviceServiceWrapper , IGattCharacteristicsResultWrapper > Services { get ; }

        /// <summary>
        ///     Defines a provider for push-based notification about Gatt services
        ///     being refreshed.
        /// </summary>
        IObservable < GattCommunicationStatus > Refreshed { get ; }


        /// <summary>
        ///     Gets the Gatt communication connection status of the device.
        /// </summary>
        GattCommunicationStatus GattCommunicationStatus { get ; }

        /// <summary>
        ///     Refresh the Gatt services.
        /// </summary>
        /// <returns>
        /// </returns>
        Task Refresh ( ) ;
    }
}