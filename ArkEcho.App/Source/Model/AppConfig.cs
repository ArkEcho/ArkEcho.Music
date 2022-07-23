using ArkEcho.Core;

namespace ArkEcho.App
{
    public class AppConfig : JsonBase
    {
        public AppConfig(string fileName) : base(fileName) { }

        [JsonProperty]
        public string ServerAddress { get; set; } = string.Empty;

        [JsonProperty]
        public bool Compression { get; set; } = false;

        [JsonProperty]
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Important;
    }
}