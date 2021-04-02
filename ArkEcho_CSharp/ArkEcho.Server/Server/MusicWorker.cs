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

        private List<string> getAllFilesSubSearch(string DirectoryPath, List<string> FileExtensionFilter)
        {            
            List<string> results = new List<string>();

            List<string> filesInDirectory = Directory.GetFiles(DirectoryPath).ToList();
            results.AddRange(filesInDirectory.FindAll(x => FileExtensionFilter.Find(y => $".{y}".Equals(Path.GetExtension(x), StringComparison.OrdinalIgnoreCase)) != null));

            foreach (string subdirectory in Directory.GetDirectories(DirectoryPath))
                results.AddRange(getAllFilesSubSearch(subdirectory, FileExtensionFilter));

            return results;
        }

        private void MusicWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            MusicLibrary library = new MusicLibrary();

            foreach (string FilePath in getAllFilesSubSearch(MusicDirectoryPath, Resources.SupportedFileFormats))
            {
                AlbumArtist albumArtist = null;
                Album album = null;

                using (TagLib.File tagFile = TagLib.File.Create(FilePath))
                {
                    MusicFile music = new MusicFile(FilePath)
                    {
                        Title = tagFile.Tag.Title,
                        Performer = tagFile.Tag.FirstPerformer,
                        Disc = tagFile.Tag.Disc,
                        Track = tagFile.Tag.Track,
                        Year = tagFile.Tag.Year,
                    };

                    if (tagFile.Tag.FirstAlbumArtist != null && tagFile.Tag.Album != null)
                    {
                        albumArtist = library.AlbumArtists.Find(x => x.Name.Equals(tagFile.Tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase));
                        if (albumArtist == null)
                        {
                            albumArtist = new AlbumArtist() { Name = tagFile.Tag.FirstAlbumArtist };
                            library.AlbumArtists.Add(albumArtist);
                        }

                        album = library.Album.Find(x => x.Name.Equals(tagFile.Tag.Album, StringComparison.OrdinalIgnoreCase));
                        if (album == null)
                        {
                            album = new Album() { AlbumArtist = albumArtist.GUID, Name = tagFile.Tag.Album };
                            library.Album.Add(album);

                            albumArtist.AlbumID.Add(album.GUID);
                        }

                        music.Album = album.GUID;
                        music.AlbumArtist = albumArtist.GUID;

                        if (music.Disc > album.DiscCount)
                            album.DiscCount = music.Disc;

                        if (music.Track > album.TrackCount)
                            album.TrackCount = music.Track;

                        album.MusicFiles.Add(music.GUID);
                        albumArtist.MusicFileIDs.Add(music.GUID);
                    }

                    library.MusicFiles.Add(music);
                }
            }

            // TODO: Media Player Playlist parsen und in neues Format

            e.Result = library;
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
