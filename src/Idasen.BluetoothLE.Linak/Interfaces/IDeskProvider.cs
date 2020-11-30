using System ;

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
    }
}