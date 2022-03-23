using System.Globalization ;
using Idasen.RESTAPI.Desk.Emulator.Dtos ;
using Idasen.RESTAPI.Desk.Emulator.Interfaces ;
using Microsoft.AspNetCore.Mvc ;

namespace Idasen.RESTAPI.Desk.Emulator.Controllers ;

[ Route ( "desk/" ) ]
public class DeskController : ControllerBase
{
    private readonly IRestDesk                  _desk ;
    private readonly ILogger < DeskController > _logger ;

    public DeskController ( ILogger < DeskController > logger ,
                            IRestDesk                  desk )
    {
        _logger = logger ;
        _desk   = desk ;
    }

    [ Route ( "" ) ]
    [ HttpGet ]
    public IActionResult GetDesk ( )
    {
        _logger.LogInformation ( "DeskController.GetDesk()" ) ;

        var dto = new DeskDto
                  {
                      BluetoothAddress     = _desk.BluetoothAddress.ToString ( CultureInfo.InvariantCulture ) ,
                      BluetoothAddressType = _desk.BluetoothAddressType ,
                      DeviceName           = _desk.DeviceName ,
                      Name                 = _desk.Name
                  } ;

        return Ok ( dto ) ;
    }

    [ Route ( "height" ) ]
    [ HttpGet ]
    public IActionResult GetHeight ( )
    {
        _logger.LogInformation ( "DeskController.GetHeight()" ) ;

        var dto = new HeightDto { Height = _desk.Height } ;

        return Ok ( dto ) ;
    }

    [ Route ( "height" ) ]
    [ HttpPost ]
    public async Task < IActionResult > SetHeight ( [ FromBody ] HeightDto dto )
    {
        _logger.LogInformation ( $"DeskController.SetHeight({dto})" ) ;

        await _desk.MoveToAsync ( dto.Height )
                   .ConfigureAwait ( false ) ;

        return Ok ( _desk.Height ) ;
    }

    [ Route ( "speed" ) ]
    [ HttpGet ]
    public IActionResult GetSpeed ( )
    {
        _logger.LogInformation ( "DeskController.GetSpeed()" ) ;

        var dto = new SpeedDto
                  {
                      Speed = _desk.Speed
                  } ;

        return Ok ( dto ) ;
    }

    [ Route ( "heightandspeed" ) ]
    [ HttpGet ]
    public IActionResult GetHeightAndSpeed ( )
    {
        _logger.LogInformation ( "DeskController.GetHeightAndSpeed()" ) ;

        var dto = new HeightAndSpeedDto
                  {
                      Height = _desk.Height ,
                      Speed  = _desk.Speed
                  } ;

        return Ok ( dto ) ;
    }

    [ Route ( "up" ) ]
    [ HttpPost ]
    public async Task < IActionResult > Up ( )
    {
        _logger.LogInformation ( "DeskController.Up()" ) ;

        await _desk.MoveUpAsync ( )
                   .ConfigureAwait ( false ) ;

        return Ok ( ) ;
    }

    [ Route ( "down" ) ]
    [ HttpPost ]
    public async Task < IActionResult > Down ( )
    {
        _logger.LogInformation ( "DeskController.Down()" ) ;

        await _desk.MoveDownAsync ( )
                   .ConfigureAwait ( false ) ;

        return Ok ( ) ;
    }

    [ Route ( "stop" ) ]
    [ HttpPost ]
    public async Task < IActionResult > Stop ( )
    {
        _logger.LogInformation ( "DeskController.Stop()" ) ;

        await _desk
             .MoveStopAsync ( )
             .ConfigureAwait ( false ) ;

        return Ok ( ) ;
    }
}