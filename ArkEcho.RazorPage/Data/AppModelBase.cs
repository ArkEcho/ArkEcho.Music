using ArkEcho.Core;
using ArkEcho.WebPage;

namespace ArkEcho.RazorPage
{
    public abstract class AppModelBase : IAppModel
    {
        public MusicLibrary Library { get; protected set; }

        public abstract Player Player { get; }

        protected Rest rest = null;
        protected LibarySync syncBase = null;
        protected Logger logger = null;

        private string appName = string.Empty;
        private Authentication authentication = null;

        public AppModelBase(string appName, ILocalStorage localStorage, RestLoggingWorker loggingWorker, string serverAddress, bool compression)
        {
            this.appName = appName;
            this.logger = new Logger(appName, "AppModel", loggingWorker);
            Library = new MusicLibrary();
            rest = new Rest(serverAddress, compression);
            authentication = new Authentication(localStorage, rest);
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

        public abstract Task<bool> InitializeLibraryAndPlayer();
        public abstract Task<string> GetAlbumCover(Guid albumGuid);
    }
}
