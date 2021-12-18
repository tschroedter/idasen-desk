using Microsoft.AspNetCore.Mvc ;

namespace Idasen.RestApi.Controllers
{
    public class Desk : ControllerBase
    {
        public Desk ( )
        {
            
        }

        [Route( "height")]
        public IActionResult Get()
        {
            return Ok("123");
        }
    }
}