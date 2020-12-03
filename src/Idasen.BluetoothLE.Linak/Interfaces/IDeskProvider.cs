using System ;
using System.Threading ;
using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskProvider
        : IDisposable
    {
        /// todo
        IObservable < IDesk > DeskDetected { get ; }

        /// todo
        IDeskProvider Initialize ( ) ;

        /// todo
        IDeskProvider StartDetecting ( ) ;

        /// todo
        IDeskProvider StopDetecting() ;

        /// <inheritdoc />
        Task<(bool, IDesk)> TryGetDesk(CancellationToken token) ;
    }
}