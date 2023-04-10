using Microsoft.AspNetCore.Mvc;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : BaseController
    {
        public ControlController() : base("Control")
        {
        }

        [HttpGet()]
        [HttpPost()]
        [Route("")]
        public ActionResult Standard()
        {
            return Ok();
        }

        //// GET: api/Control/ReloadMusicLibrary
        //[HttpGet("LoadMusicLibrary")]
        //public void ControlLoadMusicLibrary()
        //{
        //    Server.LoadMusicLibrary();
        //}

        //// GET: api/Control/StopServer
        //[HttpGet("StopServer")]
        //public void ControlStopServer()
        //{
        //    Server.Stop();
        //}

        //// GET: api/Control/RestartServer
        //[HttpGet("RestartServer")]
        //public void ControlRestartServer()
        //{
        //    Server.Restart();
        //}

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
