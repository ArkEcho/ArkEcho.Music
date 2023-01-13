namespace ArkEcho.RazorPage.Data
{
    public class AppEnvironment
    {
        public bool Development { get; set; } = false;

        public Resources.Platform Platform { get; set; } = Resources.Platform.None;

        public AppEnvironment(bool development, Resources.Platform platform)
        {
            Development = development;
            Platform = platform;
        }
    }
}
