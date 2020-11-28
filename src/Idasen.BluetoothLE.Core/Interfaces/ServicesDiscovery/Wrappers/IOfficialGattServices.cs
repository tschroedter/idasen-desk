using System ;
using System.Collections.Generic ;
using Idasen.BluetoothLE.Core.ServicesDiscovery ;

namespace Idasen.BluetoothLE.Core.Interfaces.ServicesDiscovery.Wrappers
{
    /// <summary>
    ///     Known list of Gatt Services.
    ///     "GATT Services are collections of characteristics and relationships
    ///     to other services that encapsulate the behavior of part of a device.
    ///     All Service Assigned Numbers values on this page are normative.All
    ///     other materials contained on this page is informative only.
    ///     Authoritative compliance information is contained in the applicable
    ///     Bluetooth® specification.
    ///     (see https://www.bluetooth.com/specifications/gatt/services/)
    /// </summary>
    public interface IOfficialGattServices
        : IReadOnlyCollection < OfficialGattService >
    {
        /// <summary>
        ///     Try to find a known Gatt service by Uuid.
        /// </summary>
        /// <param name="guid">
        ///     The UUID.
        /// </param>
        /// <param name="gattService">
        ///     The Gatt service or null.
        /// </param>
        /// <returns>
        ///     'true' if a matching Gatt service exists, otherwise 'false'.
        /// </returns>
        bool TryFindByUuid ( Guid                    guid ,
                             out OfficialGattService gattService ) ;
    }
}