using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Factory to create instances of <see cref="IGattDeviceServicesResultWrapper" />.
    /// </summary>
    public interface IGattDeviceServicesResultWrapperFactory
    {
        /// <summary>
        ///     Create a new instance of <see cref="IGattDeviceServicesResultWrapper" />
        ///     using the given instance of <see cref="GattDeviceServicesResult" />.
        /// </summary>
        /// <param name="result">
        ///     The <see cref="GattDeviceServicesResult" /> to be wrapped.
        /// </param>
        /// <returns>
        ///     A wrapped <see cref="GattDeviceServicesResult" />.
        /// </returns>
        IGattDeviceServicesResultWrapper Create ( [ NotNull ] GattDeviceServicesResult result ) ;
    }
}