using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui.Data
{
    public class MauiPlatformController : PlatformControllerBase
    {
        private IMauiHelper mauiHelper;

        public MauiPlatformController(IMauiHelper mauiHelper)
        {
            this.mauiHelper = mauiHelper;
        }
        public override async Task ProcessUserLogin()
        {
            mauiHelper.SetDragArea(false);
        }

        public override async Task ProcessUserLogout()
        {
            mauiHelper.SetDragArea(true);
        }
    }
}
