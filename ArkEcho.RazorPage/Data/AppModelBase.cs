using ArkEcho.Core;
using ArkEcho.WebPage;

namespace ArkEcho.RazorPage
{
    public abstract class AppModelBase : IAppModel
    {
        public MusicLibrary Library { get; protected set; }

        public abstract Player Player { get; }

        protected Rest rest = null;
        private Authentication authentication = null;

        public AppModelBase(ILocalStorage localStorage, string serverAddress, bool compression)
        {
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
