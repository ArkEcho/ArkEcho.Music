using ArkEcho.Core;

namespace ArkEcho.RazorPage.Data
{
    public class RazorConfig : JsonBase
    {
        public RazorConfig(string fileName) : base(fileName) { }
        // TODO: Prop für DarkMode
        [JsonProperty]
        public string ServerAddress { get; set; } = "https://192.168.178.20:5002";

        [JsonProperty]
        public bool Compression { get; set; } = false;

        [JsonProperty]
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Important;

        [JsonProperty]
        public Uri MusicFolder { get; set; } = new Uri("about:blank");
    }
}
