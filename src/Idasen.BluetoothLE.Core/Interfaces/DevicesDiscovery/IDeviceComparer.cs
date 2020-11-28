using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    public interface IDeviceComparer
    {
        /// <summary>
        ///     Checks if the given devices are equivalent.
        ///     Two devices are equivalent if they have the same Address.
        /// </summary>
        /// <param name="deviceA">
        ///     The first device.
        /// </param>
        /// <param name="deviceB">
        ///     The second device.
        /// </param>
        /// <returns>
        ///     'true' if the devices are equivalent, otherwise 'false'.
        /// </returns>
        [ UsedImplicitly ]
        bool IsEquivalentTo ( [ CanBeNull ] IDevice deviceA ,
                              [ CanBeNull ] IDevice deviceB ) ;

        /// <summary>
        ///     Checks if the given devices have the same property values.
        /// </summary>
        /// <param name="deviceA">
        ///     The first device.
        /// </param>
        /// <param name="deviceB">
        ///     The second device.
        /// </param>
        /// <returns>
        ///     'true' if the devices have the same property values, otherwise 'false'.
        /// </returns>
        [ UsedImplicitly ]
        bool Equals ( [ CanBeNull ] IDevice deviceA ,
                      [ CanBeNull ] IDevice deviceB ) ;
    }
}