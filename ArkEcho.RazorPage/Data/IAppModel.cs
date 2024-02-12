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

        Status AppStatus { get; }

        event Action StatusChanged;

        Task LogoutUser();

        Task<bool> InitializeOnLoad();
        Task<bool> InitializeOnLogin();
        Task<bool> LoadLibraryFromServer();
        Task<bool> UpdateMusicRating(Guid guid, int rating);
    }
}