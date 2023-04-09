using ArkEcho.Core;

namespace ArkEcho.Maui
{
    public interface IMauiHelper
    {
        string GetPlatformSpecificMusicFolder(User user);
        Task<string> PickFolder();

        string ApplicationTitle { get; set; }
        bool DarkMode { set; }
        void SetDragArea(bool fullTitlebar);
    }
}
