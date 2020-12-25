using System ;
using System.Threading ;
using System.Threading.Tasks ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskProvider
        : IDisposable
    {
        /// <summary>
        ///     Fires when a desk was detected.
        /// </summary>
        IObservable < IDesk > DeskDetected { get ; }

        /// <summary>
        ///     Initialize the instance and is required to be called first.
        /// </summary>
        /// <returns>
        ///     Returns itself.
        /// </returns>
        IDeskProvider Initialize ( ) ;

        /// <summary>
        ///     Start the desk detection process.
        /// </summary>
        /// <returns>
        ///     Returns itself.
        /// </returns>
        IDeskProvider StartDetecting ( ) ;

        /// <summary>
        ///     Stops the desk detection process.
        /// </summary>
        /// <returns>
        ///     Returns itself.
        /// </returns>
        IDeskProvider StopDetecting() ;

        /// <summary>
        ///     Try to detect a desk until the token has been cancelled.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>
        ///     A tuple with the first value indicating if a desk was found or not.
        ///     The second parameter is the detected desk or null.
        /// </returns>
        Task<(bool, IDesk)> TryGetDesk(CancellationToken token) ;

        /// <summary>
        ///     The currently detected desk.
        /// </summary>
        [CanBeNull] public IDesk Desk { get; }
    }
}