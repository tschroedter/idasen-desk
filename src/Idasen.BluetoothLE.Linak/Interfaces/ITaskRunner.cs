using System ;
using System.Threading ;
using System.Threading.Tasks ;

namespace Idasen.BluetoothLE.Linak.Interfaces
{
    public interface ITaskRunner
    {
        Task Run ( Action            action ,
                   CancellationToken token ) ;
    }
}