using Microsoft.AspNetCore.Mvc;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Instance;

        // GET: api/Control/ReloadMusicLibrary
        [HttpGet("LoadMusicLibrary")]
        public void ControlLoadMusicLibrary()
        {
            server.LoadMusicLibrary();
        }

        // GET: api/Control/StopServer
        [HttpGet("StopServer")]
        public void ControlStopServer()
        {
            server.Stop();
        }

        // GET: api/Control/RestartServer
        [HttpGet("RestartServer")]
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
