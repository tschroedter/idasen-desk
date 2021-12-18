using System.Threading.Tasks ;
using AutoMapper ;
using Idasen.Dapr.Dtos ;
using Microsoft.AspNetCore.Mvc ;
using Microsoft.Extensions.Logging ;

namespace Idasen.Dapr.Controllers
{
    [ Route ( "desk/" ) ]
    public class DeskController : ControllerBase
    {
        public DeskController ( ILogger < DeskController > logger ,
                                IMapper                    mapper ,
                                IDeskManager               manager )
        {
            _logger  = logger ;
            _mapper  = mapper ;
            _manager = manager ;
        }

        [ Route ( "" ) ]
        public IActionResult GetDesk ( )
        {
            _logger.LogInformation ( "" ) ;
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            var dto = _mapper.Map < DeskDto > ( _manager.Desk ) ;

            return Ok ( dto ) ;
        }

        [ Route ( "height" ) ]
        public IActionResult GetHeight ( )
        {
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            var dto = new HeightDto { Height = _manager.Desk.Height } ;

            return Ok ( dto ) ;
        }

        [ Route ( "height" ) ]
        [ HttpPost ]
        public async Task < IActionResult > SetHeight ( [ FromBody ] HeightDto dto )
        {
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            await _manager.Desk.MoveToAsync ( dto.Height ) ;

            return Ok ( _manager.Desk.Height ) ;
        }

        [ Route ( "speed" ) ]
        public IActionResult GetSpeed ( )
        {
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            var dto = new SpeedDto
                      {
                          Speed = _manager.Desk.Speed
                      } ;

            return Ok ( dto ) ;
        }

        [ Route ( "heightandspeed" ) ]
        public IActionResult GetHeightAndSpeed ( )
        {
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            var dto = new HeightAndSpeedDto
                      {
                          Height = _manager.Desk.Height ,
                          Speed  = _manager.Desk.Speed
                      } ;

            return Ok ( dto ) ;
        }

        [ Route ( "up" ) ]
        [ HttpPost ]
        public async Task < IActionResult > Up ( )
        {
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            await _manager.Desk.MoveUpAsync ( ) ;

            return Ok ( ) ;
        }

        [ Route ( "down" ) ]
        [ HttpPost ]
        public async Task < IActionResult > Down ( )
        {
            if ( ! _manager.IsReady )
                return StatusCode ( 500 ,
                                    "DeskManger isn't ready" ) ;

            await _manager.Desk.MoveDownAsync ( ) ;

            return Ok ( ) ;
        }

        private readonly ILogger < DeskController > _logger ;

        private readonly IDeskManager _manager ;
        private readonly IMapper      _mapper ;
    }
}