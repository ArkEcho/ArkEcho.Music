using Android.App;
using Android.OS;
using ArkEcho.App.Connection;
using ArkEcho.Core;
using ArkEcho.Player;
using System;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    public class AppModel
    {
        public static AppModel Instance { get; } = new AppModel();

        public AppConfig Config { get; private set; } = null;

        public ArkEchoRest Rest { get; private set; } = null;

        public ArkEchoVLCPlayer Player { get; private set; } = null;
        public MusicLibrary Library { get; private set; } = null;

        private PowerManager powerManager = null;
        private PowerManager.WakeLock wakeLock = null;

        private const string configFileName = "AppConfig.json";
        private const string libraryFileName = "MusicLibrary.json";

        private AppModel()
        {
            // TODO: Dispose
        }

        public async Task<bool> Init(PowerManager powerManager)
        {
            // Config and Rest
            Config = new AppConfig(configFileName);
            await Config.LoadFromFile(GetAndroidInternalPath());

            if (string.IsNullOrEmpty(Config.ServerAddress))
                Config.ServerAddress = "https://192.168.178.20:5001/api";

            await Config.SaveToFile(GetAndroidInternalPath());

            Rest = new Connection.ArkEchoRest(Config.ServerAddress);

            // Library
            Library = new MusicLibrary(libraryFileName);
            await Library.LoadFromFile(GetAndroidInternalPath());

            // Player
            Player = new Player.ArkEchoVLCPlayer();
            Player.InitPlayer(Log);

            // Create Wake Lock
            this.powerManager = powerManager;
            wakeLock = powerManager.NewWakeLock(WakeLockFlags.Full, "ArkEchoLock");

            return true;
        }

        public void PreventLock()
        {
            wakeLock.Acquire();
        }
        public void AllowLock()
        {
            wakeLock.Release();
        }

        public bool Log(string Text, Resources.LogLevel Level)
        {
            // TODO
            return true;
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

        public async Task<bool> SetMusicLibrary(string libraryString)
        {
            Library = new MusicLibrary(libraryFileName);

            bool result = await Library.LoadFromJsonString(libraryString);
            result &= await Library.SaveToFile(GetAndroidInternalPath());

            return result;
        }
    }
}