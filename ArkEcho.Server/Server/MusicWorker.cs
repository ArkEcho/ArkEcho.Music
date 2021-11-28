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

        private void MusicWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string MusicDirectoryPath = (string)e.Argument;
            if (string.IsNullOrEmpty(MusicDirectoryPath))
            {
                e.Result = null;
                return;
            }

            MusicLibrary library = new MusicLibrary();
            List<string> errors = new List<string>();

            foreach (string filePath in getAllFilesSubSearch(MusicDirectoryPath, Resources.SupportedFileFormats, errors))
            {
                AlbumArtist albumArtist = null;
                Album album = null;

                using (TagLib.File tagFile = TagLib.File.Create(filePath))
                {
                    MusicFile music = new MusicFile(filePath)
                    {
                        Title = tagFile.Tag.Title,
                        Performer = tagFile.Tag.FirstPerformer,
                        Disc = tagFile.Tag.Disc,
                        Track = tagFile.Tag.Track,
                        Year = tagFile.Tag.Year,
                    };

                    if (!checkFolderStructureAndTags(music, tagFile.Tag.Album, tagFile.Tag.FirstAlbumArtist))
                        continue;

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

                    library.MusicFiles.Add(music);
                }
            }

            if (errors.Count > 0)
                Console.WriteLine($"There were {errors.Count} on parsing the Music Folder!");

            // TODO: Media Player Playlist parsen und in neues Format

            e.Result = library;
        }

        private List<string> getAllFilesSubSearch(string DirectoryPath, List<string> FileExtensionFilter, List<string> ErrorFolder)
        {
            List<string> results = new List<string>();

            try
            {
                List<string> filesInDirectory = Directory.GetFiles(DirectoryPath).ToList();
                results.AddRange(filesInDirectory.FindAll(x => FileExtensionFilter.Find(y => $".{y}".Equals(Path.GetExtension(x), StringComparison.OrdinalIgnoreCase)) != null));

                foreach (string subdirectory in Directory.GetDirectories(DirectoryPath))
                    results.AddRange(getAllFilesSubSearch(subdirectory, FileExtensionFilter, ErrorFolder));
            }
            catch (Exception ex)
            {
                ErrorFolder.Add(ex.Message);
            }

            return results;
        }

        private bool checkFolderStructureAndTags(MusicFile music, string albumName, string albumArtistName)
        {
            if (string.IsNullOrEmpty(albumArtistName) || string.IsNullOrEmpty(albumName))
            {
                Console.WriteLine($"Skipped! No Album/AlbumArtist {music.GetFullPathWindows()}");
                return false; ;
            }
            else
            {
                List<string> parts = music.GetFullPathWindows().Split("\\").ToList();
                if (!parts[parts.Count - 3].Equals(albumArtistName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Skipped! AlbumArtist != Foldername {music.GetFullPathWindows()}");
                    return false;
                }

                if (!parts[parts.Count - 2].Equals(albumName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Skipped! Albumname != Foldername {music.GetFullPathWindows()}");
                    return false;
                }
            }
            return true;
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
