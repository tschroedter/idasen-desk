using System ;
using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Linak.Control
{
    /// <summary>
    ///     The desk doesn't always provide the current height so we have to
    ///     move the desk to get a reading.
    /// </summary>
    public interface IInitialHeightProvider
        : IDisposable
    {
        void                 Initialize ( ) ;

        /// <summary>
        ///     Start the process of checking and getting the current height
        ///     of the desk.
        /// </summary>
        /// <returns>
        ///     A Task.
        /// </returns>
        Task                 Start ( ) ;

        /// <summary>
        ///     Notifies listeners when the provider was able to determine the
        ///     height of the desk.
        /// </summary>
        IObservable < uint > Finished { get ; }

        /// <summary>
        ///     The current height of the desk.
        /// </summary>
        uint Height { get ; }
    }
}