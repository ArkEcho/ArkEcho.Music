using Android.App;
using Android.Content;
using Android.OS;
using ArkEcho.Core;
using System;
using System.Collections.Generic;
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

        private RestLoggingWorker loggingWorker = null;
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

            loggingWorker = new RestLoggingWorker(rest, (Logging.LogLevel)config.LogLevel);
            loggingWorker.RunWorkerAsync();

            logger = new Logger("App", "Main", loggingWorker);

            string configString = await config.SaveToJsonString();
            logger.LogStatic($"App Configuration:");
            logger.LogStatic($"\r\n{configString}");

            // Library
            Library = new MusicLibrary(libraryFileName);
            await Library.LoadFromFile(GetAndroidInternalPath());

            // TODO: Save Path in Library on serializing? Set on CheckLIbrary Function, what if user tries to start not loaded File?
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

            bool checkLib = await CheckLibraryWithLocalFolder(null, exist, missing);
        }

        public async Task<bool> LoadLibraryFromServer(Logging.LoggingDelegate logInListView)
        {
            try
            {
                string libraryString = await rest.GetMusicLibrary();
                if (string.IsNullOrEmpty(libraryString))
                {
                    logInListView?.Invoke("No response from the Server!", Logging.LogLevel.Static);
                    return false;
                }

                Library = new MusicLibrary(libraryFileName);

                bool result = await Library.LoadFromJsonString(libraryString);
                result &= await Library.SaveToFile(GetAndroidInternalPath());

                if (!result)
                {
                    logInListView?.Invoke("Cant load json!", Logging.LogLevel.Error);
                    return false;
                }

                logInListView?.Invoke($"Music File Count: {Library.MusicFiles.Count.ToString()}", Logging.LogLevel.Debug);

                await Task.Delay(200);
                return true;
            }
            catch (Exception ex)
            {
                logInListView?.Invoke($"Exception loading MusicLibrary: {ex.Message}", Logging.LogLevel.Error);
                return false;
            }
        }

        public async Task<bool> LoadFileFromServer(MusicFile file, Logging.LoggingDelegate logInListView)
        {
            if (file == null)
                return false;

            try
            {
                byte[] fileBytes = await rest.GetMusicFile(file.GUID);

                if (fileBytes.Length == 0)
                    return false;

                using (FileStream stream = new FileStream(file.GetFullPathAndroid(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                return File.Exists(file.GetFullPathAndroid());
            }
            catch (Exception ex)
            {
                logInListView?.Invoke($"Exception loading MusicFile {file.Title}: {ex.Message}", Logging.LogLevel.Error);
                return false;
            }
        }

        public async Task<bool> CheckLibraryWithLocalFolder(Logging.LoggingDelegate logInListView, List<MusicFile> exist, List<MusicFile> missing)
        {
            bool success = false;
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (MusicFile file in Library.MusicFiles)
                    {
                        string folder = getMusicFileFolder(file, Library);
                        if (string.IsNullOrEmpty(folder))
                        {
                            logInListView?.Invoke($"Error building Path for {file.FileName}", Logging.LogLevel.Error);
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
                    logInListView?.Invoke($"Exception loading MusicFiles: {ex.Message}", Logging.LogLevel.Error);
                }
            }
            );

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
                    //Log($"Exception loading MusicFiles: {ex.Message}", Logging.LogLevel.Error);
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