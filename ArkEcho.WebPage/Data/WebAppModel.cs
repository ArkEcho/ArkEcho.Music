using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
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
