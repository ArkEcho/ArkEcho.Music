using ArkEcho.Core;

namespace ArkEcho.Maui
{
    public interface IMauiHelper
    {
        string GetPlatformSpecificMusicFolder(User user);
        Task<string> PickFolder();
        void SetTitle(string title);
        void SetDarkMode(bool darkMode);
        void SetDragArea(bool fullTitlebar);
    }
}
