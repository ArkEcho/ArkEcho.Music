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
    public class MusicController : BaseController
    {
        public MusicController() : base("Music")
        {
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

        // GET: api/Music/Library
        [HttpGet("Library")]
        public async Task<ActionResult> GetLibraryGuid()
        {
            MusicLibrary library = Server.Instance.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return Ok(library.GUID.ToString());
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

        // GET: api/Music/MusicFiles/CountIndex
        [HttpGet("MusicFiles/{countIndex}")]
        public async Task<ActionResult> GetMusicLibrary(int countIndex)
        {
            MusicLibrary library = Server.Instance.GetMusicLibrary();
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
            MusicLibrary library = Server.Instance.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.Album);
        }

        // GET: api/Music/AlbumArtists
        [HttpGet("AlbumArtists")]
        public async Task<ActionResult> GetAlbumArtistsList(int countIndex)
        {
            MusicLibrary library = Server.Instance.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.AlbumArtists);
        }

        // GET: api/Music/Playlists
        [HttpGet("Playlists")]
        public async Task<ActionResult> GetPlaylistsList(int countIndex)
        {
            MusicLibrary library = Server.Instance.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            return await GetByteResult(library.Playlists);
        }

        private async Task<ActionResult> GetByteResult(object toSerialize)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            MusicLibrary library = Server.Instance.GetMusicLibrary();
            if (library == null)
                return BadRequest();

            byte[] data = await Serializer.Serialize(toSerialize);

            sw.Stop();
            Logger.LogImportant($"{Request.Path} took {sw.ElapsedMilliseconds}ms");

            FileContentResult result = new FileContentResult(data, "application/octet-stream");

            return result;
        }

        [HttpPost("Rating")]
        public async Task<ActionResult> UpdateMusicRating(Guid guid, int rating)
        {
            if (guid == Guid.Empty)
            {
                Logger.LogImportant($"{Request.Path} Bad Request, Guid Empty!");
                return BadRequest();
            }

            return Ok();
        }
    }
}
