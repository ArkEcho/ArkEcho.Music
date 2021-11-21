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

        public ArkEchoRest Rest { get; private set; } = null;

        public ArkEchoVLCPlayer Player { get; private set; } = null;
        public MusicLibrary Library { get; private set; } = null;

        private PowerManager powerManager = null;
        private PowerManager.WakeLock wakeLock = null;

        private AppModel()
        {
            // TODO: Dispose
        }

        public async Task<bool> Init(PowerManager powerManager)
        {
            // TODO: From App.Config
            string url = "https://192.168.178.20:5001/api";
            //string url = "https://arkecho.de/api";

            Rest = new Connection.ArkEchoRest(url);

            Player = new Player.ArkEchoVLCPlayer();
            Player.InitPlayer(Log);

            // Create Wake Lock
            this.powerManager = powerManager;
            wakeLock = powerManager.NewWakeLock(WakeLockFlags.Full, "ArkEchoLock");

            await Task.Delay(5);
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

        public bool SetMusicLibrary(string libraryString)
        {
            Library = new MusicLibrary();
            return Library.LoadFromJsonString(libraryString);
        }
    }
}