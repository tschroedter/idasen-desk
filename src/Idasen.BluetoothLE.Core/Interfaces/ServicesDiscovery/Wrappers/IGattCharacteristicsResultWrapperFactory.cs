using Windows.Devices.Bluetooth.GenericAttributeProfile ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Factory to create <see cref="IGattCharacteristicsResultWrapper" /> instances.
    /// </summary>
    public interface IGattCharacteristicsResultWrapperFactory
    {
        /// <summary>
        ///     Create a <see cref="IGattCharacteristicsResultWrapper" /> instance using
        ///     the given <see cref="GattCharacteristicsResult" /> instance.
        /// </summary>
        /// <param name="result">
        ///     The instance to wrap.
        /// </param>
        /// <returns>
        ///     The wrapper.
        /// </returns>
        IGattCharacteristicsResultWrapper Create ( [ NotNull ] GattCharacteristicsResult result ) ;
    }
}