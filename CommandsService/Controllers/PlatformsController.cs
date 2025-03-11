using Microsoft.AspNetCore.Mvc;

namespace CommandsService.AddControllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        public PlatformsController()
        {
            
        }
        [HttpPost]
        public ActionResult TestInboundConnection()
        {
            Console.WriteLine("---> Inbound Post #Command Service");
            return Ok("Inbound test of from Platforms Controller");
        }
    }
}