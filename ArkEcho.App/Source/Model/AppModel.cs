using Android.App;
using Android.Content;
using Android.OS;
using ArkEcho.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            logger = new Logger(ArkEcho.Resources.ARKECHOAPP, "Main", RestLoggingWorker);

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

            //bool checkLib = await CheckLibraryWithLocalFolder(exist, missing);

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
                logger.LogImportant($"Loading MusicLibrary took {sw.ElapsedMilliseconds}ms, MusicFiles {Library.MusicFiles.Count.ToString()}, Playlists {Library.Playlists.Count.ToString()}");

                await Task.Delay(200);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"Exception loading MusicLibrary: {ex.Message}");
                return false;
            }
        }

        private static string GetAndroidMediaAppSDFolderPath()
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

        public void PreventLock()
        {
            wakeLock.Acquire();
        }

        public void AllowLock()
        {
            wakeLock.Release();
        }

        public static string GetAndroidInternalPath()
        {
            return Application.Context.FilesDir.Path;
        }
    }
}