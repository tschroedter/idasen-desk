using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery
{
    /// <summary>
    ///     Factory to create <see cref="IDevice" /> instances.
    /// </summary>
    public interface IDeviceFactory
    {
        /// <summary>
        ///     Create a <see cref="IDevice" /> instance using the given address.
        /// </summary>
        /// <param name="address">
        ///     The address of the device.
        /// </param>
        /// <returns>
        ///     The device belonging to the address.
        /// </returns>
        Task < IDevice > FromBluetoothAddressAsync ( ulong address ) ;
    }
}