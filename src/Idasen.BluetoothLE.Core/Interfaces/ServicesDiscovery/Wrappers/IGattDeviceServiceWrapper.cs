using System ;
using System.Threading.Tasks ;
using Windows.Devices.Bluetooth.GenericAttributeProfile ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Wrapper for <see cref="GattDeviceService" />.
    /// </summary>
    public interface IGattDeviceServiceWrapper
        : IDisposable
    {
        /// <summary>
        ///     The Gatt Device Service UUID.
        /// </summary>
        Guid Uuid { get ; }

        /// <summary>
        ///     The device id the Gatt Service belongs to.
        /// </summary>
        string DeviceId { get ; }

        /// <summary>
        ///     Get all the Gatt Characteristics of the Gatt Service,
        /// </summary>
        /// <returns></returns>
        Task < IGattCharacteristicsResultWrapper > GetCharacteristicsAsync ( ) ;
    }
}