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
        void Initialize ( [ NotNull ] string deviceName ,
                          ulong              deviceAddress ) ;

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