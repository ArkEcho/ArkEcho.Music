using ArkEcho.Core;
using PlaylistsNET.Content;
using PlaylistsNET.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ArkEcho.Server
{
    public class MusicLibraryWorker : BackgroundWorker
    {
        public int ID { get; private set; }

        private Logger logger = null;

        public bool Busy { get; set; } = false;

        public MusicLibraryWorker(int id, Logger logger) : base()
        {
            ID = id;
            this.logger = logger;
            DoWork += MusicLibraryWorker_DoWork;
        }

        private void MusicLibraryWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            logger.LogDebug($"{ID}: Start loading MusicLibrary");

            Stopwatch sw = new();
            sw.Start();

            var data = (MusicLibraryManager.LibraryData)e.Argument;

            data.Library = new();

            foreach (string filePath in getAllFilesSubSearch(data.Path, Resources.SupportedMusicFileFormats))
                loadMusicFile(filePath, data.Library);

            loadPlaylistFiles(data.Path, data.Library);

            sw.Stop();
            logger.LogDebug($"{ID}: Finished loading MusicLibrary in {sw.ElapsedMilliseconds}ms, {data.Library.MusicFiles.Count} Music Files & {data.Library.Playlists.Count} Playlists");

            e.Result = data;
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
            logger.LogError($"{ID}: Unknown Playlist Format {playlist.FileFormat}");
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
                    logger.LogError($"{ID}: Error parsing Playlist {playlist.Title}, {info.ToString()} not found!");
            }
        }

        private void loadMusicFile(string filePath, MusicLibrary library)
        {
            using (TagLib.File tagFile = TagLib.File.Create(filePath))
            {
                if (tagFile == null)
                {
                    logger.LogError($"{ID}: Couldn't load Tags for {filePath}");
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

                }
                catch (Exception ex)
                {
                    logger.LogError($"{ID}: Exception on creating File for {filePath}, {ex.Message}");
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
                    byte[] coverData = tagFile.Tag.Pictures[0].Data.Data;
                    try
                    {
                        album.Cover64 = resizeImage(coverData);
                    }
                    catch (ArgumentException arg)
                    {
                        logger.LogError($"Error loading Image for Album {album.Name}, just try it");
                        album.Cover64 = Convert.ToBase64String(coverData);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Exception converting Image for Album {album.Name}:\r\n{ex.GetFullMessage()}");
                    }

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

        private string resizeImage(byte[] data)
        {
            string result = string.Empty;
            if (data.Length == 0)
                return result;

            using (MemoryStream ms = new MemoryStream(data))
            using (Bitmap bmp = new Bitmap(ms))
            {
                if ((bmp.Width >= bmp.Height - 10 && bmp.Width <= bmp.Height + 10) && data.Length < 100000) // Width/Height within 10px and original Image < 100kb
                    return Convert.ToBase64String(ms.ToArray());

                // From Stackoverflow ;)
                var destRect = new Rectangle(0, 0, Resources.ImageSize, Resources.ImageSize);
                var destImage = new Bitmap(Resources.ImageSize, Resources.ImageSize);

                destImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

                using (var graphics = Graphics.FromImage(destImage))
                {
                    graphics.CompositingMode = CompositingMode.SourceCopy;
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        graphics.DrawImage(bmp, destRect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                using (MemoryStream export = new MemoryStream())
                {
                    destImage.Save(export, ImageFormat.Png);
                    result = Convert.ToBase64String(export.ToArray());
                    Debug.WriteLine($"Image from {data.Length / 1000}kb to {export.Length / 1000}kb");
                }
            }

            return result;
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
                logger.LogError($"{ID}: Error reading File Info for {directoryPath}, {ex.Message}");
            }

            return results;
        }

        private bool checkFolderStructureAndTags(MusicFile music, TagLib.Tag tag)
        {
            if (string.IsNullOrEmpty(tag.FirstAlbumArtist) || string.IsNullOrEmpty(tag.Album))
            {
                logger.LogError($"{ID}: Skipped! No Album/AlbumArtist {music.FullPath}");
                return false;
            }
            else if (tag.Pictures.Length == 0)
            {
                logger.LogError($"{ID}: File has no Album Cover! {music.FullPath}");
                return false;
            }
            else
            {
                List<string> parts = music.FullPath.Split("\\").ToList();
                if (!parts[parts.Count - 3].Equals(tag.FirstAlbumArtist, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogError($"{ID}: Skipped! AlbumArtist != Foldername {music.FullPath}");
                    return false;
                }

                if (!parts[parts.Count - 2].Equals(tag.Album, StringComparison.OrdinalIgnoreCase))
                {
                    logger.LogError($"{ID}: Skipped! Albumname != Foldername {music.FullPath}");
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
