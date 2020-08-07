using ArkEcho.Core;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public class MusicWorker : BackgroundWorker
    {
        public MusicWorker(): base()
        {
            DoWork += MusicWorker_DoWork;
        }

        public bool Init(string MusicDirectoryPath)
        {
            if (string.IsNullOrEmpty(MusicDirectoryPath))
                return false;

            this.MusicDirectoryPath = MusicDirectoryPath;

            Initialized = true;
            return Initialized;
        }

        private void MusicWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            List<AlbumArtist> artists = new List<AlbumArtist>();
            List<Album> albums = new List<Album>();
            List<MusicFile> list = new List<MusicFile>();

            // TODO: Subdirectories 
            foreach (string FilePath in Directory.EnumerateFiles(MusicDirectoryPath))
            {
                AlbumArtist albumArtist = null;
                Album album = null;

                using (TagLib.File tagFile = TagLib.File.Create(FilePath))
                {
                    albumArtist = artists.Find(x => x.Name.Equals(tagFile.Tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase));
                    if (albumArtist == null)
                    {
                        albumArtist = new AlbumArtist() { Name = tagFile.Tag.FirstAlbumArtist };
                        artists.Add(albumArtist);
                    }

                    album = albums.Find(x => x.Name.Equals(tagFile.Tag.Album, StringComparison.OrdinalIgnoreCase));
                    if (album == null)
                    {
                        album = new Album() { AlbumArtist = albumArtist.ID, Name = tagFile.Tag.Album };
                        albums.Add(album);

                        albumArtist.AlbumID.Add(album.ID);
                    }

                    MusicFile music = new MusicFile(FilePath)
                    {
                        Album = album.ID,
                        AlbumArtist = albumArtist.ID,

                        Title = tagFile.Tag.Title,
                        Performer = tagFile.Tag.FirstPerformer,
                        Disc = tagFile.Tag.Disc,
                        Track = tagFile.Tag.Track,
                        Year = tagFile.Tag.Year
                    };
                                      
                    list.Add(music);

                    if (music.Disc > album.DiscCount)
                        album.DiscCount = music.Disc;
                    if (music.Track > album.TrackCount)
                        album.TrackCount = music.Track;

                    album.MusicFiles.Add(music.ID);
                    albumArtist.MusicFileIDs.Add(music.ID);
                }
            }

            // TODO: Media Player Playlist parsen und in neues Format

            e.Result = list;
        }

        public string MusicDirectoryPath { get; private set; } = string.Empty;

        public bool Initialized { get; private set; } = false;

        bool disposed = false;

        protected override void Dispose(bool Disposing)
        {
            if(!disposed)
            {
                if(Disposing)
                {

                }
            }
            base.Dispose(Disposing);
        }
    }
}
