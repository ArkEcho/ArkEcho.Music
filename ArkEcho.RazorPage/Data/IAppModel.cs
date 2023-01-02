using ArkEcho.Core;

namespace ArkEcho.RazorPage
{
    public interface IAppModel
    {
        MusicLibrary Library { get; }
        Player Player { get; }
        LibrarySync Sync { get; }
        RazorConfig Config { get; }

        Task<bool> AuthenticateUser(string username, string password);
        Task<bool> IsUserAuthenticated();
        Task LogoutUser();

        Task<bool> InitializeLibraryAndPlayer();
        Task<string> GetAlbumCover(Guid albumGuid);

        Task StartSynchronizeMusic();
    }
}