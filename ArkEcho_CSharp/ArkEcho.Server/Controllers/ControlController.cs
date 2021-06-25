using Microsoft.AspNetCore.Mvc;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Instance;

        // GET: api/Control/Stop
        [HttpGet("Stop")]
        public void ControlStopServer()
        {
            server.Stop();
        }

        // GET: api/Control/Restart
        [HttpGet("Restart")]
        public void ControlRestartServer()
        {
            server.Restart();
        }

        // GET: api/Control/[ID]
        [HttpGet("{id}")]
        public ActionResult ControlServer(int id)
        {
            switch (id)
            {
                case 1:
                    break;
                default:
                    return BadRequest();
            }
            return Ok();
        }
    }
}
