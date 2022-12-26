using ArkEcho.RazorPage;

namespace ArkEcho.Maui
{
    public class MauiAppConfig : RazorConfig
    {
        public MauiAppConfig(string fileName) : base(fileName)
        {
        }

        [JsonProperty]
        public Uri MusicFolder { get; set; } = new Uri("about:blank");
    }
}
