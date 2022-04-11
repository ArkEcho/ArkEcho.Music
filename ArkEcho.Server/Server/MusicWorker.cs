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
        private Logger logger = null;

        public MusicWorker(LoggingWorker lw) : base()
        {
            logger = new Logger("Server", "MusicWorker", lw);
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
                        Duration = Convert.ToInt64(tagFile.Properties.Duration.TotalMilliseconds)
                    };

                    if (!checkFolderStructureAndTags(music, tagFile.Tag))
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

                        using (MemoryStream ms = new MemoryStream(tagFile.Tag.Pictures[0].Data.Data))
                            album.Cover64 = Convert.ToBase64String(ms.ToArray());

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
                logger.LogError($"There were {errors.Count} on parsing the Music Folder!");

            // TODO: Media Player Playlist parsen und in neues Format
            // TODO: WMP Listen Upload, Eigene Playlists erstellen speichern verwalten

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

        private bool checkFolderStructureAndTags(MusicFile music, TagLib.Tag tag)
        {
            long maxDuration = 20 * 60 * 1000; // Max 20min

            if (string.IsNullOrEmpty(tag.FirstAlbumArtist) || string.IsNullOrEmpty(tag.Album))
            {
                logger.LogError($"Skipped! No Album/AlbumArtist {music.GetFullPathWindows()}");
                return false;
            }
            else if (tag.Pictures.Length == 0)
            {
                logger.LogError($"File has no Album Cover! {music.GetFullPathWindows()}");
                return false;
            }
            else if (music.Duration > maxDuration) // TODO: Alle Musik längen
            {
                logger.LogError($"Skipped! File Duration is longer than 20min max! {music.GetFullPathWindows()}");
                return false;
            }
            else
            {
                List<string> parts = music.GetFullPathWindows().Split("\\").ToList();
                if (!parts[parts.Count - 3].Equals(tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogError($"Skipped! AlbumArtist != Foldername {music.GetFullPathWindows()}");
                    return false;
                }

                if (!parts[parts.Count - 2].Equals(tag.Album, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogError($"Skipped! Albumname != Foldername {music.GetFullPathWindows()}");
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
