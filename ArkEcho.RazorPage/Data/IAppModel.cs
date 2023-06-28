using ArkEcho.Core;

namespace ArkEcho.RazorPage.Data
{
    public interface IAppModel
    {
        enum Status
        {
            Started = 0,

            NotConnected = 10,
            Connected = 20,

            LoadingLibrary = 20,

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
        Task<bool> UpdateMusicRating(Guid guid, int rating);

        void SetSnackbarDialogService(SnackbarDialogService snackbarDialogService);
        Task StartSynchronizeMusic();
        Task<bool> ChangeMusicFolder();
    }
}