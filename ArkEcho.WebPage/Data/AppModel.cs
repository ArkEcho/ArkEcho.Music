using ArkEcho.Core;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class AppModel
    {
        // TODO: private?
        public MusicLibrary Library { get; private set; } = null;

        public Rest Rest { get; private set; } = null;

        public JSPlayer Player { get; set; } = null;

        private bool initialized = false;
        private Authentication authentication = null;

        public AppModel(IJSRuntime jsRuntime, ILocalStorage localStorage)
        {
            Library = new MusicLibrary();
            Player = new JSPlayer(jsRuntime, WebPageManager.Instance.Config.ServerAddress);
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

                if (Player.InitPlayer())
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
