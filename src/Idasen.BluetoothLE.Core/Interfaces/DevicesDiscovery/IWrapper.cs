using System ;
using Idasen.BluetoothLE.Core.DevicesDiscovery ;

namespace Idasen.BluetoothLE.Core.Interfaces.DevicesDiscovery
{
    // ReSharper disable once InconsistentNaming
    public interface IWrapper
        : IDisposable
    {
        /// <summary>
        ///     Status.
        /// </summary>
        Status Status { get ; }

        /// <summary>
        ///     Fired when a device is updated.
        /// </summary>
        IObservable < IDevice > Received { get ; }

        /// <summary>
        ///     Fired when the watcher stops listening.
        /// </summary>
        IObservable < DateTime > Stopped { get ; }

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