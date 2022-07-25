using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : ArkEchoController
    {
        public MusicController() : base("Music")
        {
        }

        // GET: api/Music
        [HttpGet]
        public async Task<ActionResult> GetMusicLibrary()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string lib = await Server.GetMusicLibraryString();

            if (Server.Config.Compression)
                lib = await ZipCompression.ZipToBase64(lib);
            else
                lib = lib.ToBase64();

            sw.Stop();
            Logger.LogImportant($"{Request.Path} took {sw.ElapsedMilliseconds}ms");

            return Ok(lib);
        }

        // GET: api/Music/[GUID]
        [HttpGet("{guid}")]
        public async Task<ActionResult> GetMusicFile(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request!");
                return BadRequest();
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            MusicFile musicFile = Server.GetMusicFile(guid);

            if (musicFile == null)
                return BadRequest();

            byte[] content = await System.IO.File.ReadAllBytesAsync(musicFile.FullPath);
            if (Server.Config.Compression)
                content = await ZipCompression.Zip(content);

            FileContentResult result = new FileContentResult(content, $"application/{musicFile.FileFormat}");
            result.FileDownloadName = Path.GetFileName(musicFile.FileName);

            sw.Stop();
            Logger.LogImportant($"{Request.Path} took {sw.ElapsedMilliseconds}ms");

            return result;
        }

        // GET: api/Music/AlbumCover/[GUID]
        [HttpGet("AlbumCover/{guid}")]
        public async Task<ActionResult> GetAlbumCover(Guid guid)
        {
            if (guid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Guid Empty!");
                return BadRequest();
            }

            string cover = Server.GetAlbumCover(guid);

            if (string.IsNullOrEmpty(cover))
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Cover is Empty!");
                return BadRequest();
            }

            return Ok(cover);
        }
    }
}
