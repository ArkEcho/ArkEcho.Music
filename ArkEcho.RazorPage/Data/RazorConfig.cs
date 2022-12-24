using ArkEcho.Core;

namespace ArkEcho.RazorPage
{
    public class RazorConfig : JsonBase
    {
        public RazorConfig(string fileName) : base(fileName) { }
        // TODO: Prop für DarkMode
        [JsonProperty]
        public string ServerAddress { get; set; } = "https://192.168.178.32:5002";

        [JsonProperty]
        public bool Compression { get; set; } = false;

        [JsonProperty]
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Important;
    }
}
