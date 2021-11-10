using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class MusicLibrary : JsonBase
    {
        public MusicLibrary() : base() { }

        [JsonProperty]
        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

        [JsonProperty]
        public List<Album> Album { get; set; } = new List<Album>();

        [JsonProperty]
        public List<MusicFile> MusicFiles { get; set; } = new List<MusicFile>();

        [JsonProperty]
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
