using ArkEcho.Core;

namespace ArkEcho.Maui
{
    public interface IMauiHelper
    {
        string GetPlatformSpecificMusicFolder(User user);
        Task<string> PickFolder();
    }
}
