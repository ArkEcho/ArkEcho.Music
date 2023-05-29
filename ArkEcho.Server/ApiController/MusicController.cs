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

        // GET: api/Music/[GUID]
        [HttpGet("{musicFileGuid}")]
        public async Task<ActionResult> GetMusicFile(Guid musicFileGuid)
        {
            if (musicFileGuid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request!");
                return BadRequest();
            }

            MusicFile musicFile = server.GetMusicFile(musicFileGuid);

            if (musicFile == null)
                return BadRequest();

            byte[] content = await System.IO.File.ReadAllBytesAsync(musicFile.FullPath);

            FileContentResult result = new FileContentResult(content, $"application/{musicFile.FileFormat}");
            result.FileDownloadName = Path.GetFileName(musicFile.FileName);

            return result;
        }

        // GET: api/Music/Library
        [HttpGet("Library")]
        public async Task<ActionResult> GetLibraryGuid()
        {
            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return Ok(library.GUID.ToString());
        }

        // GET: api/Music/AlbumCover/[GUID]
        [HttpGet("AlbumCover/{albumGuid}")]
        public async Task<ActionResult> GetAlbumCover(Guid albumGuid)
        {
            if (albumGuid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Guid Empty!");
                return BadRequest();
            }

            string cover = server.GetAlbumCover(albumGuid);

            if (string.IsNullOrEmpty(cover))
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Cover is Empty!");
                return BadRequest();
            }

            return Ok(cover);
        }

        // GET: api/Music/MusicFiles/CountIndex
        [HttpGet("MusicFiles/{countIndex}")]
        public async Task<ActionResult> GetMusicLibrary(int countIndex)
        {
            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            int start = countIndex * Resources.RestMusicFileCount;
            int count = Resources.RestMusicFileCount;

            if (start >= library.MusicFiles.Count)
                return BadRequest();
            else if (start + Resources.RestMusicFileCount > library.MusicFiles.Count)
                count = library.MusicFiles.Count - start;

            return await GetByteResult(library.MusicFiles.GetRange(start, count));
        }

        // GET: api/Music/Albums
        [HttpGet("Albums")]
        public async Task<ActionResult> GetAlbumList(int countIndex)
        {
            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.Album);
        }

        // GET: api/Music/AlbumArtists
        [HttpGet("AlbumArtists")]
        public async Task<ActionResult> GetAlbumArtistsList(int countIndex)
        {
            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.AlbumArtists);
        }

        [HttpPost("Rating/{musicGuid};{rating}")]
        public async Task<ActionResult> UpdateMusicRating(Guid musicGuid, int rating)
        {
            if (musicGuid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Guid Empty!");
                return BadRequest();
            }
            else if (rating < 0 || rating > 5)
                return BadRequest();

            return server.UpdateMusicRating(musicGuid, rating) ? Ok() : NotFound();
        }

        // GET: api/Music/Playlists
        [HttpGet("Playlists")]
        public async Task<ActionResult> GetPlaylistsList()
        {
            MusicLibrary library = server.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.Playlists);
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
