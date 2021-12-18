using System.Threading.Tasks ;

namespace Idasen.Dapr
{
    public interface IDeskManager
    {
        bool          IsReady { get ; }
        IRestDesk     Desk    { get ; }
        Task < bool > Initialise ( ) ;
    }
}