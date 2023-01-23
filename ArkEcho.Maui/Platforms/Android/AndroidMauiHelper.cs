using ArkEcho.Core;

namespace ArkEcho.Maui.AndroidMaui
{
    public class AndroidMauiHelper : IMauiHelper
    {
        public string GetPlatformSpecificMusicFolder(User user)
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

        public Task<string> PickFolder()
        {
            throw new NotImplementedException();
        }
    }
}
