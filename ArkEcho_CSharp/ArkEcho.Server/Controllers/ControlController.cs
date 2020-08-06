using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class ControlController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Instance;

        [HttpGet("Stop")]
        public void ControlStopServer()
        {
            //server.Stop();
        }

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
