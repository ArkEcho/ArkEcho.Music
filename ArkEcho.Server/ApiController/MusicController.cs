using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ControllerBase
    {
        ArkEchoServer server = ArkEchoServer.Instance;

        // GET: api/Music
        [HttpGet]
        public async Task<ActionResult> GetMusicLibrary()
        {
            string lib = await server.GetMusicLibraryString();

            if (server.ServerConfig.Compression)
                lib = await ZipCompression.ZipToBase64(lib);
            else
                lib = lib.ToBase64();

            return Ok(lib);
        }

        // GET: api/Music/[GUID]
        [HttpGet("{guid}")]
        public async Task<ActionResult> GetMusicFile(Guid guid)
        {
            // TODO: Logging!
            if (guid == Guid.Empty)
                return BadRequest();

            MusicFile musicFile = server.GetMusicFile(guid);

            if (musicFile == null)
                return BadRequest();

            byte[] content = await System.IO.File.ReadAllBytesAsync(musicFile.GetFullPathWindows());
            if (server.ServerConfig.Compression)
                content = await ZipCompression.Zip(content);

            FileContentResult result = new FileContentResult(content, $"application/{musicFile.FileFormat}");
            result.FileDownloadName = Path.GetFileName(musicFile.FileName);

            return result;
        }
    }
}
