using System.Threading.Tasks ;

namespace Idasen.RESTAPI
{
    public interface IDeskManager
    {
        bool          IsReady { get ; }
        IRestDesk     Desk    { get ; }
        Task < bool > Initialise ( ) ;
    }
}