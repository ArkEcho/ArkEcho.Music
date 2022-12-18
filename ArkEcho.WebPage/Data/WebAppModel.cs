using ArkEcho.Core;
using ArkEcho.RazorPage;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class WebAppModel : IAppModel
    {
        // TODO: private?
        public MusicLibrary Library { get; private set; } = null;

        public Rest Rest { get; } = null;

        public Player Player { get { return jsPlayer; } }

        private bool initialized = false;
        private Authentication authentication = null;

        private JSPlayer jsPlayer = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage)
        {
            Library = new MusicLibrary();
            jsPlayer = new JSPlayer(jsRuntime, WebPageManager.Instance.Config.ServerAddress);
            Rest = new Rest(WebPageManager.Instance.Config.ServerAddress, WebPageManager.Instance.Config.Compression);
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

        public async Task<bool> InitializeLibraryAndPlayer()
        {
            if (initialized)
                return true;

            string lib = await Rest.GetMusicLibrary();
            await Library.LoadFromJsonString(lib);

            if (Library.MusicFiles.Count > 0)
            {
                Console.WriteLine($"AppModel initialized, {Library.MusicFiles.Count}");

                if (jsPlayer.InitPlayer())
                {
                    initialized = true;
                    return true;
                }
            }

            Console.WriteLine($"Error initializing AppModel");
            return false;
        }
    }
}
