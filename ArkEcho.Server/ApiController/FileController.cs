using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    // TODO: Alle Controller Abstrahieren für Unit Test -> Umkehr Basis Klasse als eigentliche Implementierung
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : BaseController
    {
        public FileController(Server server) : base(server, "File")
        {

        }

        // GET: api/File/ChunkTransfer?file=[Guid]&chunk=[Guid]&apiToken=[Guid]
        [HttpGet("ChunkTransfer")]
        public async Task<ActionResult> GetFileChunkTransfer([FromQuery] Guid musicFile, [FromQuery] Guid fileChunk, [FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return BadRequest();

            if (musicFile == Guid.Empty || fileChunk == Guid.Empty)
            {
                Logger.LogError($"{Request.Path} is invalid!");
                return BadRequest();
            }

            List<TransferFileBase> files = server.GetAllFiles(apiToken);

            if (files == null)
            {
                Logger.LogError($"No Files found, Library null?");
                return BadRequest();
            }

            TransferFileBase tfb = files.Find(x => x.GUID == musicFile);
            if (tfb == null)
            {
                Logger.LogError($"File requested not found!");
                return BadRequest();
            }

            TransferFileBase.FileChunk fileChunkObj = tfb.Chunks.Find(x => x.GUID == fileChunk);
            if (fileChunkObj == null)
            {
                Logger.LogError($"Chunk requested not found!");
                return BadRequest();
            }

            byte[] data = new byte[fileChunkObj.Size];
            using (FileStream fs = new FileStream(tfb.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Position = fileChunkObj.Position;
                await fs.ReadAsync(data, 0, fileChunkObj.Size);
            }

            FileContentResult result = new FileContentResult(data, "application/stream");
            return result;
        }
    }
}
