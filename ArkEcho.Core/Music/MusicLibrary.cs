using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ArkEcho.Core
{
    public class MusicLibrary
    {
        public MusicLibrary() { }

        [JsonInclude]
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

        [JsonInclude]
        public List<Album> Album { get; set; } = new List<Album>();

        [JsonInclude]
        public List<MusicFile> MusicFiles { get; set; } = new List<MusicFile>();

        [JsonInclude]
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
