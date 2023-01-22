using ArkEcho.Core;

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
    }
}
