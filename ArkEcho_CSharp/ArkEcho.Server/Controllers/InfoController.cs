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
    public class InfoController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Server;

        [HttpGet("MusicFiles")]
        public int MusicFileCount()
        {
            return server.GetAllFiles().Count;
        }
    }
}
