namespace Idasen.RestApi.Controllers
{
    public class SayHi : ControllerBase
    {
        [Route("sayhi/{name}")]
        public IActionResult Get(string name)
        {
            return Ok($"Hello {name}");
        }
    }
}