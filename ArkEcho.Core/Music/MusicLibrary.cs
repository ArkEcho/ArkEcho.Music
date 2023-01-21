using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class MusicLibrary
    {
        public MusicLibrary() { }

        public List<AlbumArtist> AlbumArtists { get; set; } = new List<AlbumArtist>();

        public List<Album> Album { get; set; } = new List<Album>();

        public List<MusicFile> MusicFiles { get; set; } = new List<MusicFile>();

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
