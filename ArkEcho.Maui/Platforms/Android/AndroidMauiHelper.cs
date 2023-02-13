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
                    if (folder.AbsolutePath.Contains("de.arkecho.maui"))
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
