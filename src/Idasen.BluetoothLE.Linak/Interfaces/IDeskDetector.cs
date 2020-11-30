using System ;
using JetBrains.Annotations ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskDetector
        : IDisposable
    {
        /// <inheritdoc />
        IObservable < IDesk > DeskDetected { get ; }

        /// <inheritdoc />
        void Initialize ( [ NotNull ] string deviceName    = "Desk" ,
                          ulong   deviceAddress = 250635178951455 ) ;

        /// <inheritdoc />
        void Start ( ) ;

        /// <inheritdoc />
        void Stop ( ) ;
    }
}