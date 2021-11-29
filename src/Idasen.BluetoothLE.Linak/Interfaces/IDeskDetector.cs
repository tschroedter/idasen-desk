using System ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskDetector
        : IDisposable
    {
        /// <summary>
        ///     Notifies when a desk was detected.
        /// </summary>
        IObservable < IDesk > DeskDetected { get ; }

        /// <summary>
        ///     Initializes the instance with the given parameters.
        /// </summary>
        /// <param name="deviceName">
        ///     The device name to detect.
        /// </param>
        /// <param name="deviceAddress">
        ///     The device address to detect.
        /// </param>
        /// <param name="deviceTimeout">
        ///     The timeout used for monitored devices after a device expires
        ///     and is removed from the cache.
        /// </param>
        IDeskDetector Initialize ( [ NotNull ] string deviceName ,
                                    ulong              deviceAddress ,
                                    uint               deviceTimeout ) ;

        /// <summary>
        ///     Start the detection of a desk by device name or device address.
        /// </summary>
        void Start ( ) ;

        /// <summary>
        ///     Stop the detection of a desk.
        /// </summary>
        void Stop ( ) ;
    }
}