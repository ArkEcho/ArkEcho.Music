using ArkEcho.Core;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ArkEcho.Server
{
    public class MusicLibraryWorker : BackgroundWorker
    {
        private Logger logger = null;

        public MusicLibraryWorker(Logger logger) : base()
        {
            this.logger = logger;
            DoWork += MusicLibraryWorker_DoWork;
        }

        private void MusicLibraryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.LogStatic($"Start loading MusicLibrary...");

            Stopwatch sw = new();
            sw.Start();
            string musicDirectoryPath = (string)e.Argument;

            MusicLibrary library = new();
            List<string> errors = new();

            foreach (string filePath in getAllFilesSubSearch(musicDirectoryPath, Resources.SupportedMusicFileFormats))
                loadMusicFile(filePath, library);

            loadPlaylistFiles(musicDirectoryPath, library);

            sw.Stop();
            logger.LogStatic($"Finished loading MusicLibrary in {sw.ElapsedMilliseconds}ms, {library.MusicFiles.Count} Music Files & {library.Playlists.Count} Playlists");

            e.Result = library;
        }

        private void loadPlaylistFiles(string musicDirectoryPath, MusicLibrary library)
        {
            foreach (string filePath in getAllFilesSubSearch(musicDirectoryPath, Resources.SupportedPlaylistFileFormats))
            {
                Playlist playlist = new();

                if (loadPlaylist(library, filePath, playlist))
                    library.Playlists.Add(playlist);
            }
        }

        private bool loadPlaylist(MusicLibrary library, string filePath, Playlist playlist)
        {
            // TODO: Mehr Playlist Formate
            switch (playlist.FileFormat)
            {
                case "wpl":
                    WplContent content = new();
                    WplPlaylist wpl = null;

                    using (FileStream stream = new(filePath, FileMode.Open))
                        wpl = content.GetFromStream(stream);

                    playlist.Title = wpl.Title;
                    mapPlaylistEntriesToMusicFiles(playlist, wpl.GetTracksPaths(), library);

                    return playlist.MusicFiles.Count > 0;
            }
            logger.LogError($"Unknown Playlist Format {playlist.FileFormat}");
            return false;
        }

        private void mapPlaylistEntriesToMusicFiles(Playlist playlist, List<string> playlistEntries, MusicLibrary library)
        {
            foreach (string entry in playlistEntries)
            {
                FileInfo info = new(entry); // WPL saves the Paths with &ng212 statt '
                MusicFile file = library.MusicFiles.Find(y => y.FullPath.EndsWith(info.ToString().Substring(5), StringComparison.OrdinalIgnoreCase));
                if (file != null)
                    playlist.MusicFiles.Add(file.GUID);
                else
                    logger.LogError($"Error parsing Playlist {playlist.Title}, {info.ToString()} not found!");
            }
        }

        private void loadMusicFile(string filePath, MusicLibrary library)
        {
            using (TagLib.File tagFile = TagLib.File.Create(filePath))
            {
                if (tagFile == null)
                {
                    logger.LogError($"Couldn't load Tags for {filePath}");
                    return;
                }

                MusicFile music = null;

                try
                {
                    music = new(filePath)
                    {
                        Title = tagFile.Tag.Title,
                        Performer = tagFile.Tag.FirstPerformer,
                        Disc = (int)tagFile.Tag.Disc,
                        Track = (int)tagFile.Tag.Track,
                        Year = (int)tagFile.Tag.Year,
                        Duration = Convert.ToInt32(tagFile.Properties.Duration.TotalMilliseconds),
                        Rating = ShellFileAccess.GetRating(filePath),
                        Bitrate = tagFile.Properties.AudioBitrate,
                    };

                    //if (filePath.Contains("Kryptonite"))
                    //{
                    //    // TODO: Move to Unit Tests
                    //    ShellFileAccess.SetRating(filePath, 5);
                    //    ShellFileAccess.GetRating(filePath);
                    //    ShellFileAccess.SetRating(filePath, 4);
                    //    ShellFileAccess.GetRating(filePath);
                    //    ShellFileAccess.SetRating(filePath, 3);
                    //    ShellFileAccess.GetRating(filePath);
                    //    ShellFileAccess.SetRating(filePath, 2);
                    //    ShellFileAccess.GetRating(filePath);
                    //    ShellFileAccess.SetRating(filePath, 1);
                    //    ShellFileAccess.GetRating(filePath);
                    //    ShellFileAccess.SetRating(filePath, 0);
                    //    ShellFileAccess.GetRating(filePath);
                    //}
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception on creating File for {filePath}, {ex.Message}");
                    return;
                }

                if (!checkFolderStructureAndTags(music, tagFile.Tag))
                    return;

                AlbumArtist albumArtist = library.AlbumArtists.Find(x => x.Name.Equals(tagFile.Tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase));
                if (albumArtist == null)
                {
                    albumArtist = new AlbumArtist() { Name = tagFile.Tag.FirstAlbumArtist };
                    library.AlbumArtists.Add(albumArtist);
                }

                Album album = library.Album.Find(x => x.Name.Equals(tagFile.Tag.Album, StringComparison.OrdinalIgnoreCase) && x.AlbumArtist == albumArtist.GUID);
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

        private List<string> getAllFilesSubSearch(string directoryPath, List<string> fileExtensionFilter)
        {
            List<string> results = new();

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
            if (string.IsNullOrEmpty(tag.FirstAlbumArtist) || string.IsNullOrEmpty(tag.Album))
            {
                logger.LogError($"Skipped! No Album/AlbumArtist {music.FullPath}");
                return false;
            }
            else if (tag.Pictures.Length == 0)
            {
                logger.LogError($"File has no Album Cover! {music.FullPath}");
                return false;
            }
            else
            {
                List<string> parts = music.FullPath.Split("\\").ToList();
                if (!parts[parts.Count - 3].Equals(tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogError($"Skipped! AlbumArtist != Foldername {music.FullPath}");
                    return false;
                }

                if (!parts[parts.Count - 2].Equals(tag.Album, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogError($"Skipped! Albumname != Foldername {music.FullPath}");
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
