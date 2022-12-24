using Android.App;
using Android.Runtime;
using Android.Widget;

namespace ArkEcho.Maui
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            var app = MauiProgram.CreateMauiApp(MauiProgram.Platform.Android, Android.App.Application.Context.FilesDir.Path, getAndroidMediaAppSDFolderPath());
            if (app == null)
            {
                Toast mrToast = Toast.MakeText(this, "ArkEcho Maui Error on Startup!", ToastLength.Short);
                mrToast.Show();
                StopService(null);
            }
            return app;
        }

        private string getAndroidMediaAppSDFolderPath()
        {
            string baseFolderPath = string.Empty;
            try
            {
                foreach (Java.IO.File folder in Android.App.Application.Context.GetExternalMediaDirs())
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
    }
}