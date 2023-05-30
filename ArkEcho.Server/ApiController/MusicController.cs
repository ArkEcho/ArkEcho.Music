using ArkEcho.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController : BaseController
    {
        public MusicController(Server server) : base(server, "Music")
        {
        }

        // GET: api/Music?musicFile=[GUID]?apiToken=[apiToken]
        [HttpGet()]
        public async Task<ActionResult> GetMusicFile([FromQuery] Guid musicFile, [FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return BadRequest();
            else if (musicFile == Guid.Empty)
                return BadRequest();

            MusicFile file = server.GetMusicFile(musicFile);

            if (file == null)
                return NotFound();

            byte[] content = await System.IO.File.ReadAllBytesAsync(file.FullPath);

            FileContentResult result = new FileContentResult(content, $"application/{file.FileFormat}");
            result.FileDownloadName = Path.GetFileName(file.FileName);

            return result;
        }

        // GET: api/Music/Library
        [HttpGet("Library")]
        public async Task<ActionResult> GetLibraryGuid([FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return BadRequest();

            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return Ok(library.GUID.ToString());
        }

        [HttpGet("AlbumCover")]
        public async Task<ActionResult> GetAlbumCover([FromQuery] Guid albumGuid, [FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return BadRequest();

            if (albumGuid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Guid Empty!");
                return BadRequest();
            }

            string cover = server.GetAlbumCover(albumGuid);

            if (string.IsNullOrEmpty(cover))
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Cover is Empty!");
                return NotFound();
            }

            return Ok(cover);
        }

        [HttpGet("MusicFiles")]
        public async Task<ActionResult> GetMusicFiles([FromQuery] int musicFileCountIndex, [FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return Unauthorized();

            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            int start = musicFileCountIndex * Resources.RestMusicFileCount;
            int count = Resources.RestMusicFileCount;

            if (start >= library.MusicFiles.Count)
                return BadRequest();
            else if (start + Resources.RestMusicFileCount > library.MusicFiles.Count)
                count = library.MusicFiles.Count - start;

            return await GetByteResult(library.MusicFiles.GetRange(start, count));
        }

        // GET: api/Music/Albums
        [HttpGet("Albums")]
        public async Task<ActionResult> GetAlbumList([FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return Unauthorized();

            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.Album);
        }

        // GET: api/Music/AlbumArtists
        [HttpGet("AlbumArtists")]
        public async Task<ActionResult> GetAlbumArtistsList([FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return Unauthorized();

            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.AlbumArtists);
        }

        // GET: api/Music/Playlists
        [HttpGet("Playlists")]
        public async Task<ActionResult> GetPlaylistsList([FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return Unauthorized();

            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.Playlists);
        }

        [HttpPost("Rating")]
        public async Task<ActionResult> UpdateMusicRating([FromQuery] Guid musicFile, [FromQuery] int musicRating, [FromQuery] Guid apiToken)
        {
            if (!checkApiToken(apiToken))
                return Unauthorized();

            if (musicFile == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Guid Empty!");
                return BadRequest();
            }
            else if (musicRating < 0 || musicRating > 5)
                return BadRequest();

            return server.UpdateMusicRating(musicFile, musicRating) ? Ok() : NotFound();
        }

        private async Task<ActionResult> GetByteResult(object toSerialize)
        {
            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            byte[] data = await Serializer.Serialize(toSerialize);

            FileContentResult result = new FileContentResult(data, "application/octet-stream");

            return result;
        }
    }
}
