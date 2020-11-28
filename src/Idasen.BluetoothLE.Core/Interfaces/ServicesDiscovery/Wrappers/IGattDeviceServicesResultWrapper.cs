using System.Collections.Generic ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Wrapper for <see cref="GattDeviceServicesResult" />.
    /// </summary>
    public interface IGattDeviceServicesResultWrapper
    {
        /// <summary>
        ///     Gets the communication status of the operation.
        /// </summary>
        GattCommunicationStatus Status { get ; }

        /// <summary>
        ///     Gets the services.
        /// </summary>
        IEnumerable < IGattDeviceServiceWrapper > Services { get ; }

        /// <summary>
        ///     Gets the protocol error.
        /// </summary>
        byte? ProtocolError { get ; }
    }
}