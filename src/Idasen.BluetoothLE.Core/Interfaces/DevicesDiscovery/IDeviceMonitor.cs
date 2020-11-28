using System ;
using System.Collections.Generic ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    /// <summary>
    ///     Monitors discovered devices.
    /// </summary>
    public interface IDeviceMonitor
        : IDisposable
    {
        /// <summary>
        ///     Collection of discovered devices.
        /// </summary>
        public IReadOnlyCollection < IDevice > DiscoveredDevices { get ; }

        /// <summary>
        ///     Flag indicating if the watcher is listening or not.
        /// </summary>
        public bool IsListening { get ; }

        /// <summary>
        ///     Fired when a device is updated.
        /// </summary>
        public IObservable < IDevice > DeviceUpdated { get ; }

        /// <summary>
        ///     Fired when a new device is discovered.
        /// </summary>
        public IObservable < IDevice > DeviceDiscovered { get ; }

        /// <summary>
        ///     Fired when a device name is updated.
        /// </summary>
        public IObservable < IDevice > DeviceNameUpdated { get ; }

        /// <summary>
        ///     Starts listening.
        /// </summary>
        public void Start ( ) ;

        /// <summary>
        ///     Stops listening.
        /// </summary>
        public void Stop ( ) ;

        /// <summary>
        ///     Remove a device from the list of discovered devices.
        /// </summary>
        /// <param name="device"></param>
        public void RemoveDevice ( IDevice device ) ;
    }
}