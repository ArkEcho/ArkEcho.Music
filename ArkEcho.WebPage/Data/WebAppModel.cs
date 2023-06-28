using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override Player Player { get; protected set; }
        public override LibrarySync Sync { get; }

        public override string MusicFolder { get { return string.Empty; } }

        private IJSRuntime jsRuntime = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage, AppEnvironment environment)
            : base(environment, localStorage)
        {
            this.jsRuntime = jsRuntime;
        }

        protected override async Task<bool> initializePlayer()
        {
            var player = new JSPlayer(jsRuntime, logger, Environment.ServerAddress);
            Player = player;
            return ((JSPlayer)Player).InitPlayer(rest.ApiToken.ToString());
        }

        public override async Task<bool> InitializeOnLogin() => await base.InitializeOnLogin();

        public override Task StartSynchronizeMusic() => throw new NotImplementedException();

        public override Task<bool> ChangeMusicFolder() => throw new NotImplementedException();
    }
}
