using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace ArkEcho.Server
{
    public class MusicWorker : BackgroundWorker
    {
        public MusicWorker() : base()
        {
            DoWork += MusicWorker_DoWork;
        }

        private List<string> getAllFilesSubSearch(string DirectoryPath, List<string> FileExtensionFilter)
        {
            // TODO: Access Violation on AppData etc.
            List<string> results = new List<string>();

            List<string> filesInDirectory = Directory.GetFiles(DirectoryPath).ToList();
            results.AddRange(filesInDirectory.FindAll(x => FileExtensionFilter.Find(y => $".{y}".Equals(Path.GetExtension(x), StringComparison.OrdinalIgnoreCase)) != null));

            foreach (string subdirectory in Directory.GetDirectories(DirectoryPath))
                results.AddRange(getAllFilesSubSearch(subdirectory, FileExtensionFilter));

            return results;
        }

        private void MusicWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string MusicDirectoryPath = (string)e.Argument;
            if (string.IsNullOrEmpty(MusicDirectoryPath))
            {
                e.Result = null;
                return;
            }

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

                    string albumArtistName = tagFile.Tag.FirstAlbumArtist;
                    string albumName = tagFile.Tag.Album;

                    if (string.IsNullOrEmpty(albumArtistName) || string.IsNullOrEmpty(albumName))
                    {
                        Console.WriteLine($"Skipped! No Album/AlbumArtist {music.GetFullFilePath()}");
                        continue;
                    }

                    albumArtist = library.AlbumArtists.Find(x => x.Name.Equals(albumArtistName, StringComparison.OrdinalIgnoreCase));
                    if (albumArtist == null)
                    {
                        albumArtist = new AlbumArtist() { Name = albumArtistName };
                        library.AlbumArtists.Add(albumArtist);
                    }

                    album = library.Album.Find(x => x.Name.Equals(albumName, StringComparison.OrdinalIgnoreCase));
                    if (album == null)
                    {
                        album = new Album() { AlbumArtist = albumArtist.GUID, Name = albumName };
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

                    library.MusicFiles.Add(music);
                }
            }

            // TODO: Media Player Playlist parsen und in neues Format

            e.Result = library;
        }

        bool disposed = false;

        protected override void Dispose(bool Disposing)
        {
            if (!disposed)
            {
                if (Disposing)
                {

                }
            }
            base.Dispose(Disposing);
        }
    }
}
