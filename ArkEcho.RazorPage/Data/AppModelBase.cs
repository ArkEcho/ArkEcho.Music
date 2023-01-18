using ArkEcho.Core;
using ArkEcho.WebPage;

namespace ArkEcho.RazorPage.Data
{
    public abstract class AppModelBase : IAppModel
    {
        public MusicLibrary Library { get; }

        public abstract Player Player { get; }

        public abstract LibrarySync Sync { get; }

        public abstract string MusicFolder { get; }

        protected Rest rest = null;
        protected Logger logger = null;
        protected RestLoggingWorker loggingWorker = null;
        protected AppEnvironment environment = null;
        private Authentication authentication = null;

        public AppModelBase(AppEnvironment environment, ILocalStorage localStorage)
        {
            this.environment = environment;

            rest = new Rest($"https://192.168.178.20:5002", environment.UserHttpClientHandler, false);

            loggingWorker = new RestLoggingWorker(rest, Logging.LogLevel.Important);
            loggingWorker.RunWorkerAsync();

            logger = new Logger(environment.AppName, "AppModel", loggingWorker);

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

        public User GetLoggedInUser()
        {
            return authentication.AuthenticatedUser;
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

            if (Library.MusicFiles.Count > 0)
            {
                logger.LogStatic($"Library initialized, {Library.MusicFiles.Count}");
                return true;
            }
            else
            {
                logger.LogStatic($"Error initializing Library/Music Count is Zero!");
                return false;
            }
        }

        public async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return await rest.GetAlbumCover(albumGuid);
        }

        public abstract Task<bool> InitializeOnLoad();
        public abstract Task<bool> InitializeOnLogin();

        public abstract Task StartSynchronizeMusic();
    }
}
