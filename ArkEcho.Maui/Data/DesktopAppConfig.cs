using ArkEcho.RazorPage;

namespace ArkEcho.Desktop
{
    public class DesktopAppConfig : RazorConfig
    {
        public DesktopAppConfig(string fileName) : base(fileName)
        {
        }

        [JsonProperty]
        public Uri MusicFolder { get; set; } = new Uri("about:blank");
    }
}
