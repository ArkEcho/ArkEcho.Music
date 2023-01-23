using ArkEcho.Core;

namespace ArkEcho.RazorPage
{
    public interface IAppModel
    {
        MusicLibrary Library { get; }
        Player Player { get; }
        LibrarySync Sync { get; }
        string MusicFolder { get; }
        AppEnvironment Environment { get; }
        User AuthenticatedUser { get; }

        Task<bool> AuthenticateUser(string username, string password);
        Task<bool> IsUserAuthenticated();
        Task LogoutUser();

        Task<bool> InitializeOnLoad();
        Task<bool> InitializeOnLogin();
        Task<string> GetAlbumCover(Guid albumGuid);

        Task StartSynchronizeMusic();
        Task<bool> ChangeMusicFolder();
    }
}