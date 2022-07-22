using ArkEcho.Core;

namespace ArkEcho.WebPage
{
    public class WebPageConfig : JsonBase
    {
        public WebPageConfig(string fileName) : base(fileName) { }

        [JsonProperty]
        public int Port { get; set; } = 5001;

        [JsonProperty]
        public string ServerAddress { get; set; } = "https://192.168.178.20:5002";

        [JsonProperty]
        public bool Compression { get; private set; } = false;

        [JsonProperty]
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Important;
    }
}
