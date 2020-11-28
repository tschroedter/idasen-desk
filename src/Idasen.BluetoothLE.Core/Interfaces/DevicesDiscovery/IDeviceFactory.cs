using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    /// <summary>
    ///     Factory to create devices.
    /// </summary>
    public interface IDeviceFactory
    {
        /// <summary>
        ///     Create a new device with the given parameters.
        /// </summary>
        /// <param name="broadcastTime">
        ///     The time of the broadcast advertisement message of the device.
        /// </param>
        /// <param name="address">
        ///     The address of the device.
        /// </param>
        /// <param name="name">
        ///     The name of the device.
        /// </param>
        /// <param name="rawSignalStrengthInDBm">
        ///     The signal strength in dB.
        /// </param>
        /// <returns>
        ///     A new device.
        /// </returns>
        IDevice Create ( [ NotNull ] IDateTimeOffset broadcastTime ,
                         ulong                       address ,
                         [ CanBeNull ] string        name ,
                         short                       rawSignalStrengthInDBm ) ;
    }
}