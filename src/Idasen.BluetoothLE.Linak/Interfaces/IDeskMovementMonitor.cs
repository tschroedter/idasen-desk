using System ;
using Idasen.BluetoothLE.Linak.Control ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskMovementMonitor
        : IDisposable
    {
        void Initialize ( int capacity = DeskMovementMonitor.DefaultCapacity ) ;
    }
}