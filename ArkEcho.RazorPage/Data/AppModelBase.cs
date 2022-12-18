using ArkEcho.Core;
using ArkEcho.WebPage;

namespace ArkEcho.RazorPage
{
    public abstract class AppModelBase : IAppModel
    {
        public abstract MusicLibrary Library { get; }

        public abstract Player Player { get; }

        public Rest Rest { get; private set; } = null;

        private Authentication authentication = null;

        public AppModelBase(ILocalStorage localStorage, string serverAddress, bool compression)
        {
            Rest = new Rest(serverAddress, compression);
            authentication = new Authentication(localStorage, Rest);
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
    }
}
