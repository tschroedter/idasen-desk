using System ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskLocker
        : IDisposable
    {
        IDeskLocker Lock ( ) ;
        IDeskLocker Unlock ( ) ;
        IDeskLocker Initialize ( ) ;
        bool        IsLocked { get ; }
    }
}