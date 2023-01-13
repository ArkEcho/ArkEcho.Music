using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class MusicLibrary : JsonBase
    {
        public MusicLibrary() : base() { }

        public MusicLibrary(string fileName) : base(fileName) { }

        [JsonProperty]
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

        [JsonProperty]
        public List<Album> Album { get; set; } = new List<Album>();

        [JsonProperty]
        public List<MusicFile> MusicFiles { get; set; } = new List<MusicFile>();

        [JsonProperty]
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();

        public AlbumArtist GetAlbumArtist(Guid artist)
        {
            return AlbumArtists.Find(x => x.GUID == artist);
        }

        public Album GetAlbum(Guid album)
        {
            return Album.Find(x => x.GUID == album);
        }
    }
}
