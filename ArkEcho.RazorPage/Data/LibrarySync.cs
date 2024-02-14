using ArkEcho.Core;
using System.Diagnostics;

namespace ArkEcho.RazorPage.Data
{
    // TODO: Progress mit Nummer/Gesamt
    public class LibrarySync
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

        public EventHandler<ProgressEventArgs> SyncProgress;

        public string MusicFolder { get { return getUserMusicFolder(); } }

        public class LibraryCheckResult
        {
            public bool FilesMissing { get; set; }
        }

        private Rest rest = null;
        protected Logger logger = null;
        private IMauiHelper mauiHelper;
        private Authentication authentication;

        public LibrarySync(IMauiHelper mauiHelper, Authentication authentication, Rest rest, Logger logger)
        {
            this.rest = rest;
            this.logger = logger;
            this.mauiHelper = mauiHelper;
            this.authentication = authentication;
        }

        private string getUserMusicFolder()
        {
            return mauiHelper.GetPlatformSpecificMusicFolder(authentication.AuthenticatedUser);
        }

        public async Task<bool> StartSyncMusicLibrary(MusicLibrary library)
        {
            string musicFolder = getUserMusicFolder();
            if (library == null)
            {
                logger.LogError("Given library is null!");
                return false;
            }
            else if (string.IsNullOrEmpty(musicFolder))
            {
                logger.LogError($"Given MusicFolder is empty");
                return false;
            }

            if (!Directory.Exists(musicFolder))
            {
                try
                {
                    Directory.CreateDirectory(musicFolder);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception Creating MusicFolder - {ex.GetFullMessage()}");
                    return false;
                }
            }

            logger.LogImportant($"Checking Files");
            progressEvent("Checking Library", 5);

            List<MusicFile> exist = new List<MusicFile>();
            List<MusicFile> missing = new List<MusicFile>();
            List<MusicFile> wrong = new List<MusicFile>();
            bool checkLib = await CheckLibrary(musicFolder, library, exist, missing, wrong);

            if (!checkLib)
            {
                logger.LogError("Error checking the Library with the local Folder");
                return false;
            }

            if (wrong != null && wrong.Count > 0)
            {
                progressEvent($"Deleting {wrong.Count} wrong Files!", 10);
                logger.LogImportant($"Deleting {wrong.Count} wrong Files!");
                wrong.ForEach(musicFile => File.Delete(musicFile.FullPath));
                missing.AddRange(wrong);
            }

            if (missing.Count > 0)
            {
                progressEvent("Loading Missing Files", 20);
                logger.LogImportant($"Loading {missing.Count} Files");
                bool success = await loadMissingFiles(missing, exist);
            }

            progressEvent("Cleaning Up", 90);

            logger.LogImportant($"Cleaning Up");

            await cleanUpFolder(musicFolder, exist);

            logger.LogStatic($"Success!");

            progressEvent("Success!", 100);

            return true;
        }

        private void progressEvent(string message, int progress)
        {
            SyncProgress?.Invoke(this, new ProgressEventArgs(message, progress));
        }

        private async Task<bool> loadMissingFiles(List<MusicFile> missing, List<MusicFile> exist)
        {
            try
            {
                int count = 0;
                foreach (MusicFile file in missing)
                {
                    count++;
                    logger.LogDebug($"Loading {file.FileName}");

                    double test = ((double)count / missing.Count) * 50;
                    int progress = Convert.ToInt32(test + 20);
                    progressEvent($"{file.Title}\r\n{file.Performer}", progress);

                    bool success = await loadFileFromServer(file);
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

        private async Task<bool> loadFileFromServer(MusicFile file)
        {
            if (file == null)
                return false;

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                if (!Directory.Exists(file.Folder.LocalPath))
                    Directory.CreateDirectory(file.Folder.LocalPath);

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
                    file.ExistsLocally = true;
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

        public async Task<bool> CheckLibraryOnStart(MusicLibrary library, LibraryCheckResult result)
        {
            string musicFolder = getUserMusicFolder();
            if (!Directory.Exists(musicFolder))
            {
                logger.LogImportant($"Music Folder doesn't exist Folder={musicFolder}");
                result.FilesMissing = true;
                return true;
            }

            List<MusicFile> missing = new();
            if (!await CheckLibrary(musicFolder, library, new List<MusicFile>(), missing, new List<MusicFile>()))
            {
                logger.LogError($"Error checking Library on Start");
                return false;
            }

            result.FilesMissing = missing.Count > 0;

            return true;
        }

        public async Task<bool> CheckLibrary(string musicFolder, MusicLibrary library, List<MusicFile> exist, List<MusicFile> missing, List<MusicFile> wrong)
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
                            missing.Add(file);
                        else if (wrong != null && !file.TestCheckSum()) // Test Checksum -> Album Cover changed etc.
                            wrong.Add(file);
                        else
                        {
                            file.ExistsLocally = true;
                            exist.Add(file);
                        }
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
