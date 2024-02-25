using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class MusicLibrary
    {
        public MusicLibrary() { }

        [JsonInclude]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonInclude]
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

        [JsonInclude]
        public List<Album> Album { get; set; } = new List<Album>();

        [JsonInclude]
        public List<MusicFile> MusicFiles { get; set; } = new List<MusicFile>();

        [JsonInclude]
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();

        private Dictionary<Album, List<MusicFile>> albumFileMap = null;

        public AlbumArtist GetAlbumArtist(Guid artist)
        {
            return AlbumArtists.Find(x => x.GUID == artist);
        }

        public Album GetAlbum(Guid album)
        {
            return Album.Find(x => x.GUID == album);
        }

        public async Task CreateAlbumFileMap()
        {
            albumFileMap = new Dictionary<Album, List<MusicFile>>();

            await Task.Run(() =>
            {
                foreach (Album album in Album)
                {
                    List<MusicFile> files = MusicFiles.FindAll(x => x.Album == album.GUID);
                    files = files.OrderBy(x => x.Track).ThenBy(x => x.Title).ToList(); // Default sorting
                    albumFileMap.Add(album, files);
                }
            });
        }

        public List<MusicFile> GetMusicFiles(Album album)
        {
            return albumFileMap.GetValueOrDefault(album);
        }

        public object GetArtistAlbumPlaylist(Guid guid)
        {
            object result = Album.Find(x => x.GUID == guid);
            if (result != null)
                return result;
            result = AlbumArtists.Find(x => x.GUID == guid);
            if (result != null)
                return result;
            return Playlists.Find(x => x.GUID == guid);
        }
    }
}
