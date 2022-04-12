using Android.App;
using Android.Content;
using Android.OS;
using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    public class AppModel
    {
        /// <summary>
        /// SingleTon
        /// </summary>
        public static AppModel Instance { get; } = new AppModel();

        private AppConfig config = null;

        private Rest rest = null;

        public RestLoggingWorker RestLoggingWorker { get; private set; } = null;
        private Logger logger = null;

        private PowerManager powerManager = null;
        private PowerManager.WakeLock wakeLock = null;

        private const string configFileName = "AppConfig.json";
        private const string libraryFileName = "MusicLibrary.json";
        public VLCPlayer Player { get; private set; } = null;

        public MusicLibrary Library { get; private set; } = null;

        private AppModel()
        {
            // TODO: Dispose
        }

        public async Task<bool> Init(Activity activity)
        {
            // Config and Rest
            config = new AppConfig(configFileName);
            await config.LoadFromFile(GetAndroidInternalPath());

            if (string.IsNullOrEmpty(config.ServerAddress))
                config.ServerAddress = "https://192.168.178.20:5002";

            await config.SaveToFile(GetAndroidInternalPath());

            rest = new Rest(config.ServerAddress, config.Compression);

            RestLoggingWorker = new RestLoggingWorker(rest, (Logging.LogLevel)config.LogLevel);
            RestLoggingWorker.RunWorkerAsync();

            logger = new Logger("App", "Main", RestLoggingWorker);

            string configString = await config.SaveToJsonString();
            logger.LogStatic($"App Configuration:");
            logger.LogStatic($"\r\n{configString}");

            // Library
            Library = new MusicLibrary(libraryFileName);
            await Library.LoadFromFile(GetAndroidInternalPath());

            Task.Run(() => checkLibraryOnStartup());

            // Player
            Player = new VLCPlayer();
            Player.InitPlayer();

            // Create Wake Lock
            this.powerManager = (PowerManager)activity.GetSystemService(Context.PowerService);
            wakeLock = powerManager.NewWakeLock(WakeLockFlags.Full, "ArkEchoLock");

            return true;
        }

        private async Task checkLibraryOnStartup()
        {
            List<MusicFile> exist = new List<MusicFile>();
            List<MusicFile> missing = new List<MusicFile>();

            bool checkLib = await CheckLibraryWithLocalFolder(exist, missing);

            // TODO: What now
        }

        public async Task<bool> LoadLibraryFromServer()
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                string libraryString = await rest.GetMusicLibrary();
                if (string.IsNullOrEmpty(libraryString))
                {
                    logger.LogStatic("No response from the Server!");
                    return false;
                }

                Library = new MusicLibrary(libraryFileName);

                bool result = await Library.LoadFromJsonString(libraryString);
                result &= await Library.SaveToFile(GetAndroidInternalPath());

                if (!result)
                {
                    logger.LogError("Cant load json!");
                    return false;
                }

                sw.Stop();
                logger.LogImportant($"Loading MusicLibrary took {sw.ElapsedMilliseconds}ms, Music File Count: {Library.MusicFiles.Count.ToString()}");

                await Task.Delay(200);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading MusicLibrary: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> LoadFileFromServer(MusicFile file)
        {
            if (file == null)
                return false;

            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                byte[] fileBytes = await rest.GetMusicFile(file.GUID);

                if (fileBytes.Length == 0)
                    return false;

                using (FileStream stream = new FileStream(file.GetFullPathAndroid(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                sw.Stop();

                if (File.Exists(file.GetFullPathAndroid()))
                {
                    logger.LogImportant($"Success loading MusicFile in {sw.ElapsedMilliseconds}, {file.GetFullPathAndroid()}");
                    return true;
                }
                else
                {
                    logger.LogError($"Error loading MusicFile, {file.GetFullPathAndroid()}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading MusicFile {file.Title}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> CheckLibraryWithLocalFolder(List<MusicFile> exist, List<MusicFile> missing)
        {
            bool success = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (MusicFile file in Library.MusicFiles)
                    {
                        string folder = getMusicFileFolder(file, Library);
                        if (string.IsNullOrEmpty(folder))
                        {
                            logger.LogError($"Error building Path for {file.FileName}");
                            break;
                        }

                        file.Folder = new Uri(folder);

                        if (!Directory.Exists(file.Folder.LocalPath) || !File.Exists(file.GetFullPathAndroid()))
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
                    logger.LogError($"Exception loading MusicFiles: {ex.Message}");
                }
            }
            );

            logger.LogImportant($"Checking Library with Local Folder took {sw.ElapsedMilliseconds}ms");

            return success;
        }


        public async Task CleanUpFolder(string folder, List<MusicFile> okFiles)
        {
            foreach (string subFolder in Directory.GetDirectories(folder))
                await CleanUpFolder(subFolder, okFiles); // Rekursion

            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (string file in Directory.GetFiles(folder))
                    {
                        if (okFiles.Find(x => x.GetFullPathAndroid().Equals(file, StringComparison.OrdinalIgnoreCase)) == null)
                            File.Delete(file);
                    }

                    if (Directory.GetDirectories(folder).Length == 0 && Directory.GetFiles(folder).Length == 0)
                        Directory.Delete(folder);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception loading MusicFiles: {ex.Message}");
                }
            }
            );
        }

        private string getMusicFileFolder(MusicFile file, MusicLibrary lib)
        {
            string mediaFolderPath = GetAndroidMediaAppSDFolderPath();
            Album album = lib.Album.Find(x => x.GUID == file.Album);
            AlbumArtist artist = lib.AlbumArtists.Find(x => x.GUID == file.AlbumArtist);

            if (album == null || artist == null)
                return string.Empty;

            return $"{mediaFolderPath}/{artist.Name}/{album.Name}";
        }

        public void PreventLock()
        {
            wakeLock.Acquire();
        }
        public void AllowLock()
        {
            wakeLock.Release();
        }

        public static string GetAndroidMediaAppSDFolderPath()
        {
            string baseFolderPath = string.Empty;
            try
            {
                foreach (Java.IO.File folder in Application.Context.GetExternalMediaDirs())
                {
                    bool IsRemovable = Android.OS.Environment.InvokeIsExternalStorageRemovable(folder);
                    bool IsEmulated = Android.OS.Environment.InvokeIsExternalStorageEmulated(folder);

                    if (IsRemovable && !IsEmulated)
                    {
                        baseFolderPath = folder.Path;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetBaseFolderPath caused the following exception: {0}", ex);
            }

            return baseFolderPath;
        }

        public static string GetAndroidInternalPath()
        {
            return Application.Context.FilesDir.Path;
        }
    }
}