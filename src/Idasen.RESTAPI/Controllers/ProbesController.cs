using Idasen.RESTAPI.Filters ;
using Idasen.RESTAPI.Interfaces ;
using Microsoft.AspNetCore.Mvc ;

namespace Idasen.RESTAPI.Controllers
{
    [ ApiKeyAuth ]
    [ Route ( "desk/" ) ]
    public class ProbeController : ControllerBase
    {
        public ProbeController( IDeskManager               manager )
        {
            _manager    = manager ;
        }

        [ Route ("liveness") ]
        public IActionResult GetLiveness( )
        {
            return Ok ( true ) ;
        }

        [ Route ("readiness") ]
        public IActionResult GetReadiness( )
        {
            return Ok ( _manager.IsReady ) ;
        }

        private readonly IDeskManager        _manager ;
    }
}