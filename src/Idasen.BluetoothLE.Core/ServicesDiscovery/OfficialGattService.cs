namespace Idasen.BluetoothLE.Core.ServicesDiscovery
{
    /// <summary>
    ///     Details about a specific GATT Service.
    ///     (see https://www.bluetooth.com/specifications/gatt/services/)
    /// </summary>
    public class OfficialGattService
    {
        /// <summary>
        ///     The human-readable name for the service.
        /// </summary>
        public string Name { get ; set ; }

        /// <summary>
        ///     The uniform type identifier to the service.
        /// </summary>
        public string UniformTypeIdentifier { get ; set ; }

        /// <summary>
        ///     The 16-bit assigned number for this service.
        ///     The Bluetooth GATT Service UUID contains this.
        /// </summary>
        public ushort AssignedNumber { get ; set ; }

        /// <summary>
        ///     The type of specification that this service is.
        ///     (see https://www.bluetooth.com/specifications/gatt/)
        /// </summary>
        public string ProfileSpecification { get ; set ; }
    }
}