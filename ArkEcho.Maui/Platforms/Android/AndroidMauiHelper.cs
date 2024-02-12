using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui.AndroidMaui
{
    public class AndroidMauiHelper : IMauiHelper
    {
        public event Action MediaPlayPauseKeyPressed;
        public event Action MediaStopKeyPressed;
        public event Action MediaPreviousTrackKeyPressed;
        public event Action MediaNextTrackKeyPressed;

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

        public Task<string> PickFolder() { return Task.FromResult(string.Empty); }
        public bool DarkMode { get; set; }
        public void SetDragArea(bool fullTitlebar) { }
        public string ApplicationTitle { get; set; }
    }
}
