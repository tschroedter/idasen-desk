using System.Threading.Tasks ;

namespace Idasen.RESTAPI.Interfaces
{
    public interface IDeskManager
    {
        bool          IsReady { get ; }
        IRestDesk     Desk    { get ; }
        Task < bool > Initialise ( ) ;
    }
}