using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface IDeskCommandExecutor
    {
        Task < bool > Up ( ) ;
        Task < bool > Down ( ) ;
        Task < bool > Stop ( ) ;
    }
}