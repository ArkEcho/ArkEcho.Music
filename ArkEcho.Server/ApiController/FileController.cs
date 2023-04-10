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
        public FileController() : base("File")
        {

        }

        // GET: api/File/ChunkTransfer?file=[Guid]&chunk=[Guid]
        [HttpGet("ChunkTransfer")]
        public async Task<ActionResult> GetFileChunkTransfer(Guid file, Guid chunk)
        {
            if (file == Guid.Empty || chunk == Guid.Empty)
            {
                Logger.LogError($"{Request.Path} is invalid!");
                return BadRequest();
            }

            List<TransferFileBase> files = Server.GetAllFiles();

            if (files == null)
            {
                Logger.LogError($"No Files found, Library null?");
                return BadRequest();
            }

            TransferFileBase tfb = files.Find(x => x.GUID == file);
            if (tfb == null)
            {
                Logger.LogError($"File requested not found!");
                return BadRequest();
            }

            TransferFileBase.FileChunk fileChunk = tfb.Chunks.Find(x => x.GUID == chunk);
            if (fileChunk == null)
            {
                Logger.LogError($"Chunk requested not found!");
                return BadRequest();
            }

            byte[] data = new byte[fileChunk.Size];
            using (FileStream fs = new FileStream(tfb.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Position = fileChunk.Position;
                await fs.ReadAsync(data, 0, fileChunk.Size);
            }

            FileContentResult result = new FileContentResult(data, "application/stream");
            return result;
        }
    }
}
