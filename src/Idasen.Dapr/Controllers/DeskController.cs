using System.Threading.Tasks ;
using AutoMapper ;
using Idasen.Dapr.Dtos ;
using Microsoft.AspNetCore.Mvc ;

namespace Idasen.Dapr.Controllers
{
    [ Route ( "desk/" ) ]
    public class DeskController : ControllerBase
    {
        public DeskController ( IMapper      mapper ,
                                IDeskManager manager )
        {
            _mapper  = mapper ;
            _manager = manager ;
        }

        [ Route ( "/" ) ]
        public IActionResult GetDesk ( )
        {
            if ( _manager.IsReady )
            {
                var dto = _mapper.Map < DeskDto > ( _manager.Desk ) ;

                return Ok ( dto ) ;
            }

            return StatusCode ( 500 ,
                                "DeskManger isn't ready" ) ;
        }

        [ Route ( "height" ) ]
        public IActionResult GetHeight ( )
        {
            if ( _manager.IsReady )
            {
                var dto = new HeightDto { Height = _manager.Desk.Height } ;

                return Ok ( dto ) ;
            }

            return StatusCode ( 500 ,
                                "DeskManger isn't ready" ) ;
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
            if ( _manager.IsReady )
            {
                var dto = new SpeedDto
                          {
                              Speed = _manager.Desk.Speed
                          } ;

                return Ok ( dto ) ;
            }

            return StatusCode ( 500 ,
                                "DeskManger isn't ready" ) ;
        }

        [ Route ( "heightandspeed" ) ]
        public IActionResult GetHeightAndSpeed ( )
        {
            if ( _manager.IsReady )
            {
                var dto = new HeightAndSpeedDto
                          {
                              Height = _manager.Desk.Height ,
                              Speed  = _manager.Desk.Speed
                          } ;

                return Ok ( dto ) ;
            }

            return StatusCode ( 500 ,
                                "DeskManger isn't ready" ) ;
        }

        private readonly IDeskManager _manager ;

        private readonly IMapper _mapper ;
    }
}