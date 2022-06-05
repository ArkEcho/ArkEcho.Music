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
            string musicDirectoryPath = (string)e.Argument;

            MusicLibrary library = new MusicLibrary();
            List<string> errors = new List<string>();

            loadMusicFiles(musicDirectoryPath, library);
            loadPlaylistFiles(musicDirectoryPath, library);

            e.Result = library;
        }

        private void loadPlaylistFiles(string musicDirectoryPath, MusicLibrary library)
        {
            // TODO: Media Player Playlist parsen und in neues Format
            foreach (string filePath in getAllFilesSubSearch(musicDirectoryPath, Resources.SupportedPlaylistFileFormats))
            {
            }
        }

        private void loadMusicFiles(string musicDirectoryPath, MusicLibrary library)
        {
            foreach (string filePath in getAllFilesSubSearch(musicDirectoryPath, Resources.SupportedMusicFileFormats))
            {
                TagLib.File tagFile = TagLib.File.Create(filePath);
                if (tagFile == null)
                {
                    logger.LogError($"Couldn't load Tags for {filePath}");
                    continue;
                }

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
                {
                    tagFile.Dispose();
                    tagFile = null;
                    continue;
                }

                AlbumArtist albumArtist = library.AlbumArtists.Find(x => x.Name.Equals(tagFile.Tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase));
                if (albumArtist == null)
                {
                    albumArtist = new AlbumArtist() { Name = tagFile.Tag.FirstAlbumArtist };
                    library.AlbumArtists.Add(albumArtist);
                }

                Album album = library.Album.Find(x => x.Name.Equals(tagFile.Tag.Album, StringComparison.OrdinalIgnoreCase));
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

                tagFile.Dispose();
                tagFile = null;
            }
        }

        private List<string> getAllFilesSubSearch(string directoryPath, List<string> fileExtensionFilter)
        {
            List<string> results = new List<string>();

            try
            {
                List<string> filesInDirectory = Directory.GetFiles(directoryPath).ToList();
                results.AddRange(filesInDirectory.FindAll(x => fileExtensionFilter.Find(y => $".{y}".Equals(Path.GetExtension(x), StringComparison.OrdinalIgnoreCase)) != null));

                foreach (string subdirectory in Directory.GetDirectories(directoryPath))
                    results.AddRange(getAllFilesSubSearch(subdirectory, fileExtensionFilter));
            }
            catch (Exception ex)
            {
                logger.LogError($"Error reading File Info for {directoryPath}, {ex.Message}");
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
            else if (music.Duration > maxDuration) // TODO: Alle Musik längen, Timeout bei Rest, Kompression
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
