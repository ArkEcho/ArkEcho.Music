using Android.App;
using Android.Content;
using ArkEcho.App.Connection;
using ArkEcho.Core;
using ArkEcho.Player;
using System;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    public class AppModel : IDisposable
    {
        public static AppModel Instance { get; } = new AppModel();

        private ArkEchoRest rest = null;

        public ArkEchoVLCPlayer Player { get; private set; } = null;

        private AppModel()
        {
            rest = new Connection.ArkEchoRest();
            Player = new Player.ArkEchoVLCPlayer();
        }

        public async Task<bool> Init()
        {
            Player.InitPlayer(Log);
            await Task.Delay(5);
            return true;
        }

        public bool Log(string Text, Resources.LogLevel Level)
        {
            // TODO
            return true;
        }

        public static string GetMusicSDFolderPath()
        {
            string baseFolderPath = string.Empty;
            try
            {
                Context context = Application.Context;

                Java.IO.File[] dirs = context.GetExternalMediaDirs();//.GetExternalFilesDirs(null);

                foreach (Java.IO.File folder in dirs)
                {
                    bool IsRemovable = Android.OS.Environment.InvokeIsExternalStorageRemovable(folder);
                    bool IsEmulated = Android.OS.Environment.InvokeIsExternalStorageEmulated(folder);

                    if (IsRemovable && !IsEmulated)
                    {
                        baseFolderPath = folder.Path.Substring(0, folder.Path.IndexOf("Android/") + 8);
                        baseFolderPath += "Music/";
                        //break;
                    }
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine("GetBaseFolderPath caused the following exception: {0}", ex);
            }

            return baseFolderPath;
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}