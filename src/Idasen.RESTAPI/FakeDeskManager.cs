using System.Threading.Tasks ;

namespace Idasen.RESTAPI
{
    internal class FakeDeskManager : IDeskManager
    {
        public FakeDeskManager ( )
        {
            IsReady = true ;
            Desk    = new FakeDesk ( ) ;
        }

        public Task < bool > Initialise ( )
        {
            return Task.FromResult ( true ) ;
        }

        public bool      IsReady { get ; }
        public IRestDesk Desk    { get ; }
    }
}