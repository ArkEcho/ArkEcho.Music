using ArkEcho.Core;
using Windows.Storage.Pickers;

namespace ArkEcho.Maui.WinUI
{
    public class WindowsMauiHelper : IMauiHelper
    {
        public string GetPlatformSpecificMusicFolder(User user)
        {
            UserSettings.UserPath path = user.Settings.GetLocalUserSettings();
            if (path != null)
                return path.Path.LocalPath;
            else
                return string.Empty;
        }

        public async Task<string> PickFolder()
        {
            var folderPicker = new FolderPicker();
            // Make it work for Windows 10
            folderPicker.FileTypeFilter.Add("*");
            // Get the current window's HWND by passing in the Window object
            var hwnd = ((MauiWinUIWindow)App.Current.Application.Windows[0].Handler.PlatformView).WindowHandle;

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var result = await folderPicker.PickSingleFolderAsync();

            if (result == null)
                return string.Empty;

            return result.Path;
        }
    }
}
