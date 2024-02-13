namespace ArkEcho.RazorPage.Data
{
    public abstract class PlatformControllerBase
    {
        public abstract Task ProcessUserLogin();
        public abstract Task ProcessUserLogout();
    }
}
