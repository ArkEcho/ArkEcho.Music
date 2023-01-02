using ArkEcho.Core;
using ArkEcho.WebPage;

namespace ArkEcho.RazorPage
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

        public AppModelBase(string appName, ILocalStorage localStorage, RestLoggingWorker loggingWorker, RazorConfig config)
        {
            this.Config = config;
            this.appName = appName;
            this.logger = new Logger(appName, "AppModel", loggingWorker);

            rest = new Rest(config.ServerAddress, config.Compression);
            authentication = new Authentication(localStorage, rest);

            Library = new MusicLibrary();
        }

        public async Task<bool> IsUserAuthenticated()
        {
            return await authentication.GetAuthenticationState();
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            return await authentication.AuthenticateUserForLogin(username, password);
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

            return await Library.LoadFromJsonString(lib);
        }

        public abstract Task<bool> InitializeLibraryAndPlayer();
        public abstract Task<string> GetAlbumCover(Guid albumGuid);

        public abstract Task StartSynchronizeMusic();
        public abstract Task StopSynchonizeMusic();
    }
}
