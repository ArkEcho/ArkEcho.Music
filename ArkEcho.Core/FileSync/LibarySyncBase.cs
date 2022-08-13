using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    // TODO: Nicht abstract
    public abstract class LibarySyncBase
    {
        public class ProgressEventArgs
        {
            public string Message { get; set; } = string.Empty;
            public int ProgressPercent { get; set; } = 0;

            public ProgressEventArgs(string message, int progressPercent)
            {
                this.Message = message;
                this.ProgressPercent = progressPercent;
            }
        }
        public event EventHandler<ProgressEventArgs> Progress; // TODO: Progress Event

        private Rest rest = null;
        protected Logger logger = null;

        public LibarySyncBase(Rest rest)
        {
            this.rest = rest;
        }

        public async Task<bool> SyncMusicLibrary(string musicFolder, MusicLibrary library)
        {
            if (library == null)
            {
                logger.LogError("Given library is null!");
                return false;
            }
            else if (string.IsNullOrEmpty(musicFolder))
            {
                logger.LogError($"Empty MusicFolder String!");
                return false;
            }

            logger.LogImportant($"Checking Files");

            List<MusicFile> exist = new List<MusicFile>();
            List<MusicFile> missing = new List<MusicFile>();
            bool checkLib = await checkLibraryWithLocalFolder(musicFolder, library, exist, missing);

            if (!checkLib)
            {
                logger.LogError("Error checking the Library with the local Folder");
                return false;
            }

            if (missing.Count > 0)
            {
                logger.LogImportant($"Loading {missing.Count} Files");
                bool success = await loadMissingFiles(missing, exist);
            }

            logger.LogImportant($"Cleaning Up");

            await cleanUpFolder(musicFolder, exist);

            logger.LogStatic($"Success!");

            return true;
        }

        private async Task<bool> loadMissingFiles(List<MusicFile> missing, List<MusicFile> exist)
        {
            try
            {
                foreach (MusicFile file in missing)
                {
                    logger.LogDebug($"Loading {file.FileName}");

                    bool success = await LoadFileFromServer(file);
                    if (!success)
                        logger.LogError($"Error loading {file.FileName} from Server!");

                    exist.Add(file);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading MusicFiles: {ex.GetFullMessage()}");
                return false;
            }
            return true;
        }

        private async Task<bool> LoadFileFromServer(MusicFile file)
        {
            if (file == null)
                return false;

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                using (MemoryStream dataStream = await rest.GetFile(file))
                {
                    if (dataStream.Length == 0)
                    {
                        logger.LogError($"Error transfering File, Stream is empty - {file.FullPath}");
                        return false;
                    }

                    using (FileStream stream = new FileStream(file.FullPath, FileMode.OpenOrCreate,
                        FileAccess.ReadWrite, FileShare.None))
                    {
                        dataStream.Seek(0, SeekOrigin.Begin);
                        await dataStream.CopyToAsync(stream);
                    }
                }

                if (!file.TestCheckSum())
                {
                    if (File.Exists(file.FullPath))
                        File.Delete(file.FullPath);

                    logger.LogError($"Error transfering File, Checksum not okay - {file.FullPath}");
                    return false;
                }

                sw.Stop();

                if (File.Exists(file.FullPath))
                {
                    logger.LogImportant($"Success loading File in {sw.ElapsedMilliseconds}, {file.FullPath}");
                    return true;
                }
                else
                {
                    logger.LogError($"Error loading File, {file.FullPath}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading File {file.Title}: {ex.GetFullMessage()}");
                return false;
            }
        }

        private async Task<bool> checkLibraryWithLocalFolder(string musicFolder, MusicLibrary library, List<MusicFile> exist, List<MusicFile> missing)
        {
            bool success = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (MusicFile file in library.MusicFiles)
                    {
                        string folder = getMusicFileFolder(musicFolder, file, library);
                        if (string.IsNullOrEmpty(folder))
                        {
                            logger.LogError($"Error building Path for {file.FileName}");
                            break;
                        }

                        file.Folder = new Uri(folder);

                        if (!Directory.Exists(file.Folder.LocalPath) || !File.Exists(file.FullPath))
                        {
                            Directory.CreateDirectory(file.Folder.LocalPath);
                            missing.Add(file);
                        }
                        else
                            exist.Add(file);
                    }

                    success = true;
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception loading MusicFiles: {ex.GetFullMessage()}");
                }
            }
            );

            logger.LogImportant($"Checking Library with Local Folder took {sw.ElapsedMilliseconds}ms");

            return success;
        }


        private async Task cleanUpFolder(string folder, List<MusicFile> okFiles)
        {
            foreach (string subFolder in Directory.GetDirectories(folder))
                await cleanUpFolder(subFolder, okFiles); // Rekursion

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        if (okFiles.Find(x => x.FullPath.Equals(file, StringComparison.OrdinalIgnoreCase)) == null)
                            File.Delete(file);
                    }

                    if (Directory.GetDirectories(folder).Length == 0 && Directory.GetFiles(folder).Length == 0)
                        Directory.Delete(folder);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception loading MusicFiles: {ex.GetFullMessage()}");
                }
            }
            );
        }

        private string getMusicFileFolder(string musicFolder, MusicFile file, MusicLibrary library)
        {
            Album album = library.Album.Find(x => x.GUID == file.Album);
            AlbumArtist artist = library.AlbumArtists.Find(x => x.GUID == file.AlbumArtist);

            if (album == null || artist == null)
                return string.Empty;

            return $"{musicFolder}{Resources.FilePathDivider}{artist.Name}{Resources.FilePathDivider}{album.Name}";
        }
    }
}
