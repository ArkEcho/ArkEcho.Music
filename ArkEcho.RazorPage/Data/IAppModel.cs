using ArkEcho.Core;

namespace ArkEcho.RazorPage.Data
{
    public interface IAppModel
    {
        enum Status
        {
            Started = 0,

            Connecting = 10,
            Connected,
            NotConnected,

            LoadingLibrary = 20,
            LoadingAlbumCover,
            Initialized,

            Authorized = 50,
        }

        MusicLibrary Library { get; }
        Player Player { get; }
        LibrarySync Sync { get; }
        string MusicFolder { get; }
        AppEnvironment Environment { get; }
        User AuthenticatedUser { get; }
        Status AppStatus { get; }

        event Action StatusChanged;

        Task<bool> AuthenticateUser(string username, string password);
        Task<bool> IsUserAuthenticated();
        Task LogoutUser();

        Task<bool> InitializeOnLoad();
        Task<bool> InitializeOnLogin();
        Task<string> GetAlbumCover(Guid albumGuid);
        Task<bool> UpdateMusicRating(Guid guid, int rating);

        Task StartSynchronizeMusic();
        Task<bool> ChangeMusicFolder();
    }
}