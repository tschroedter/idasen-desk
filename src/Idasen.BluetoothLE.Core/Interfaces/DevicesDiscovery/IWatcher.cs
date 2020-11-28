using System ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    /// <summary>
    ///     Wraps and makes use of the <see cref="Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher" />
    ///     for easier consumption.
    /// </summary>
    public interface IWatcher
        : IDisposable
    {
        /// <summary>
        ///     Flag indicating if the watcher is listening or not.
        /// </summary>
        bool IsListening { get ; }

        /// <summary>
        ///     Fired when a device is updated.
        /// </summary>
        IObservable < IDevice > Received { get ; }

        /// <summary>
        ///     Fired when the watcher stops listening.
        /// </summary>
        IObservable < DateTime > Stopped { get ; }

        /// <summary>
        ///     Fired when the watcher starts listening.
        /// </summary>
        IObservable < DateTime > Started { get ; }

        /// <summary>
        ///     Starts listening.
        /// </summary>
        void Start ( ) ;

        /// <summary>
        ///     Stops listening.
        /// </summary>
        void Stop ( ) ;
    }
}