using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override Player Player { get; protected set; }

        private IJSRuntime jsRuntime = null;
        private BrowserCloseActionsController browserCloseActions;
        private AppEnvironment environment;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage, Rest rest, Logger logger, AppEnvironment environment, BrowserCloseActionsController browserCloseActions)
            : base(logger, rest)
        {
            this.jsRuntime = jsRuntime;
            this.browserCloseActions = browserCloseActions;
            this.environment = environment;
        }

        protected override async Task<bool> initializePlayer()
        {
            var player = new JSPlayer(jsRuntime, logger, environment.ServerAddress);
            Player = player;
            return ((JSPlayer)Player).InitPlayer(rest.ApiToken.ToString());
        }

        public override async Task<bool> InitializeOnLogin()
        {
            await browserCloseActions.SetPageExit();
            await browserCloseActions.SetMessageOnPageExit(true);
            return await base.InitializeOnLogin();
        }

        public override async Task LogoutUser()
        {
            await browserCloseActions.SetMessageOnPageExit(false);
            await base.LogoutUser();
        }
    }
}
