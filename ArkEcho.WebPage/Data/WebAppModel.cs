using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override Player Player { get { return jsPlayer; } }
        public override LibrarySync Sync { get; }

        public override string MusicFolder { get { return string.Empty; } }

        private JSPlayer jsPlayer = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage, AppEnvironment environment)
            : base(environment, localStorage)
        {
            jsPlayer = new JSPlayer(jsRuntime, logger, $"https://192.168.178.20:5002");
        }

        public async override Task<bool> InitializeOnLoad()
        {
            if (!await rest.CheckConnection())
                return false;

            //logger.LogStatic($"Executing on {environment.Platform}");

            if (!jsPlayer.InitPlayer())
                return false;

            return await LoadLibraryFromServer();
        }

        public override async Task<bool> InitializeOnLogin()
        {
            return true;
        }

        public override Task StartSynchronizeMusic()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ChangeMusicFolder()
        {
            throw new NotImplementedException();
        }
    }
}
