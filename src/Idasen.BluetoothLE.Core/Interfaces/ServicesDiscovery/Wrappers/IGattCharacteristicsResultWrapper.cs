using System.Collections.Generic ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Wrapper for <see cref="GattCharacteristicsResult" />.
    /// </summary>
    public interface IGattCharacteristicsResultWrapper
    {
        /// <summary>
        ///     Gets the status.
        /// </summary>
        GattCommunicationStatus Status { get ; }

        /// <summary>
        ///     Gets the protocol error, if there is one.
        /// </summary>
        byte? ProtocolError { get ; }

        /// <summary>
        ///     Gets the characterisitics.
        /// </summary>
        IReadOnlyList < IGattCharacteristicWrapper > Characteristics { get ; }

        /// <summary>
        ///     Initialize the instance.
        /// </summary>
        /// <returns></returns>
        Task < IGattCharacteristicsResultWrapper > Initialize ( ) ;
    }
}