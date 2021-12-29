using Idasen.RESTAPI.Filters ;
using Idasen.RESTAPI.Interfaces ;
using Microsoft.AspNetCore.Mvc ;
using Microsoft.Extensions.Logging ;

namespace Idasen.RESTAPI.Controllers
{
    [ ApiKeyAuth ]
    [ Route ( "desk/" ) ]
    public class ProbeController : ControllerBase
    {
        public ProbeController ( ILogger < ProbeController > logger ,
                                 IDeskManager                manager )
        {
            _logger  = logger ;
            _manager = manager ;
        }

        [ Route ( "liveness" ) ]
        public IActionResult GetLiveness ( )
        {
            _logger.LogInformation ( $"Liveness: {_manager.IsReady}" ) ;

            return Ok ( true ) ;
        }

        [ Route ( "readiness" ) ]
        public IActionResult GetReadiness ( )
        {
            _logger.LogInformation ( $"Readiness: {_manager.IsReady}" ) ;

            return Ok ( _manager.IsReady ) ;
        }

        private readonly ILogger < ProbeController > _logger ;
        private readonly IDeskManager                _manager ;
    }
}