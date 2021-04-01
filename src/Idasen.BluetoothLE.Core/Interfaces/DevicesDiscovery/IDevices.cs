using System.Collections.Generic ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    /// <summary>
    ///     The collection contains the discovered devices.
    /// </summary>
    public interface IDevices
    {
        /// <summary>
        ///     Collection of discovered devices.
        /// </summary>
        [ NotNull ]
        IReadOnlyCollection < IDevice > DiscoveredDevices { get ; }

        /// <summary>
        ///     Add or Update a device in the collection.
        /// </summary>
        /// <param name="device"></param>
        /// <returns>The added or updated device.</returns>
        void AddOrUpdateDevice ( [ NotNull ] IDevice device ) ;

        /// <summary>
        ///     Determines if the collection contains a specific device.
        /// </summary>
        /// <param name="device"></param>
        /// <returns>
        ///     'true' if the device was found, otherwise 'false'.
        /// </returns>
        [ UsedImplicitly ]
        bool ContainsDevice ( [ NotNull ] IDevice device ) ;

        /// <summary>
        ///     Gets the value associated with the specified key.
        /// </summary>
        /// <param name="address">The address of the device to get.</param>
        /// <param name="device">
        ///     When this method returns, contains the device associated with the specified address, if the
        ///     address is found; otherwise, the default null. This parameter is passed uninitialized.
        /// </param>
        /// <returns>true if the dictionary contains an element with the specified key; otherwise, false.</returns>
        bool TryGetDevice ( ulong                     address ,
                            [ CanBeNull ] out IDevice device ) ;

        /// <summary>
        ///     Remove the given device from the collection.
        /// </summary>
        /// <param name="device">
        ///     The device to be removed.
        /// </param>
        void RemoveDevice ( [ NotNull ] IDevice device ) ;

        /// <summary>
        ///     Clear the collection.
        /// </summary>
        void Clear ( ) ;
    }
}