using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ArkEcho.Server.ApiController
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ArkEchoController
    {
        public FileController() : base("File")
        {

        }

        // GET: api/File/ChunkTransfer?file=[Guid]&chunk=[Guid]
        [HttpGet("ChunkTransfer")]
        public async Task<ActionResult> GetFileChunkTransfer(Guid file, Guid chunk)
        {
            if (file == Guid.Empty || chunk == Guid.Empty)
                return BadRequest();



            return Ok();
        }
    }
}
