using ArkEcho.Core;
using ArkEcho.WebPage;

namespace ArkEcho.RazorPage.Data
{
    public abstract class AppModelBase : IAppModel
    {
        public MusicLibrary Library { get; }

        public abstract Player Player { get; }

        public abstract LibrarySync Sync { get; }

        public RazorConfig Config { get; }

        protected Rest rest = null;
        protected Logger logger = null;

        private string appName = string.Empty;
        private Authentication authentication = null;

        public AppModelBase(string appName, ILocalStorage localStorage, Rest rest, RestLoggingWorker loggingWorker, RazorConfig config)
        {
            this.Config = config;
            this.appName = appName;
            this.logger = new Logger(appName, "AppModel", loggingWorker);

            this.rest = rest;
            authentication = new Authentication(localStorage, rest);

            Library = new MusicLibrary();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            return await authentication.GetAuthenticationState();
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            return await authentication.AuthenticateUserForLogin(username, Encryption.EncryptSHA256(password));
        }

        public async Task LogoutUser()
        {
            if (Player.Playing)
                Player.Stop();

            await authentication.MarkUserAsLoggedOut();
        }

        protected async Task<bool> LoadLibraryFromServer()
        {
            string lib = await rest.GetMusicLibrary();
            if (string.IsNullOrEmpty(lib))
                return false;
            else if (Library != null)
            {
                string existing = await Library.SaveToJsonString();
                if (existing.Equals(lib, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            if (!await Library.LoadFromJsonString(lib))
                return false;

            foreach (Album album in Library.Album)
            {
                if (string.IsNullOrEmpty(album.Cover64))
                    album.Cover64 = await GetAlbumCover(album.GUID);
            }

            return true;
        }

        public async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return await rest.GetAlbumCover(albumGuid);
        }

        public abstract Task<bool> InitializeLibraryAndPlayer();

        public abstract Task StartSynchronizeMusic();
    }
}
