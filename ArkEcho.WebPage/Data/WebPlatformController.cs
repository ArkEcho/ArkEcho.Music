using ArkEcho.RazorPage.Data;

namespace ArkEcho.WebPage.Data
{
    public class WebPlatformController : PlatformControllerBase
    {
        private BrowserCloseActionsController browserCloseActions;

        public WebPlatformController(BrowserCloseActionsController browserCloseActions)
        {
            this.browserCloseActions = browserCloseActions;
        }

        public override async Task ProcessUserLogin()
        {
            await browserCloseActions.SetPageExit();
            await browserCloseActions.SetMessageOnPageExit(true);
        }

        public override async Task ProcessUserLogout()
        {
            await browserCloseActions.SetMessageOnPageExit(false);
        }
    }
}
