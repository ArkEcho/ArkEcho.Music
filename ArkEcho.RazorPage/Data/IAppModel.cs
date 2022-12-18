using ArkEcho.Core;

namespace ArkEcho.RazorPage
{
    public interface IAppModel
    {
        MusicLibrary Library { get; }
        Player Player { get; }
        Rest Rest { get; }

        Task<bool> AuthenticateUser(string username, string password);
        Task<bool> IsUserAuthenticated();
        Task LogoutUser();

        Task<bool> InitializeLibraryAndPlayer();
    }
}