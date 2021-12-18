using System.Threading.Tasks ;
using Idasen.BluetoothLE.Linak.Interfaces ;

namespace Idasen.Dapr
{
    // todo make IDesk methods async
    public interface IRestDesk : IDesk
    {
        uint          Height { get ; }
        int           Speed  { get ; }
        Task < bool > MoveToAsync ( uint targetHeight ) ;
        Task < bool > MoveUpAsync ( ) ;
        Task < bool > MoveDownAsync ( ) ;
        Task < bool > MoveStopAsync ( ) ;
    }
}