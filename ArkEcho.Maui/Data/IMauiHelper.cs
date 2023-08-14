using ArkEcho.Core;

namespace ArkEcho.Maui
{
    public interface IMauiHelper
    {
        public event Action MediaPlayPauseKeyPressed;
        public event Action MediaStopKeyPressed;
        public event Action MediaPreviousTrackKeyPressed;
        public event Action MediaNextTrackKeyPressed;

        string GetPlatformSpecificMusicFolder(User user);
        Task<string> PickFolder();

        string ApplicationTitle { get; set; }
        bool DarkMode { set; }
        void SetDragArea(bool fullTitlebar);
    }
}
