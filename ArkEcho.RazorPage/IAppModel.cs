using ArkEcho.Core;

namespace ArkEcho.RazorPage
{
    public interface IAppModel
    {
        MusicLibrary Library { get; }
        Player Player { get; }
        LibrarySync Sync { get; }
        string MusicFolder { get; }

        Task<bool> AuthenticateUser(string username, string password);
        Task<bool> IsUserAuthenticated();
        Task LogoutUser();
        User GetLoggedInUser();

        Task<bool> InitializeOnLoad();
        Task<bool> InitializeOnLogin();
        Task<string> GetAlbumCover(Guid albumGuid);

        Task StartSynchronizeMusic();
    }
}