using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class MusicLibrary : JsonBase
    {
        public MusicLibrary() : base() { }

        [JsonProperty]
        public List<AlbumArtist> AlbumArtists { get; private set; } = new List<AlbumArtist>();

        [JsonProperty]
        public List<Album> Album { get; private set; } = new List<Album>();

        [JsonProperty]
        public List<MusicFile> MusicFiles { get; private set; } = new List<MusicFile>();

        [JsonProperty]
        public List<Playlist> Playlists { get; private set; } = new List<Playlist>();
    }
}
