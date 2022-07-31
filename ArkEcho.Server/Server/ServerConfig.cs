using ArkEcho.Core;
using System;

namespace ArkEcho.Server
{
    public class ServerConfig : JsonBase
    {
        public ServerConfig(string FileName) : base(FileName) { }

        [JsonProperty]
        public Uri MusicFolder { get; private set; } = new Uri("about:blank");

        [JsonProperty]
        public Uri LoggingFolder { get; private set; } = new Uri("about:blank");

        [JsonProperty]
        public bool Compression { get; private set; } = false;

        [JsonProperty]
        public int Port { get; private set; } = 5002;

        [JsonProperty]
        public Logging.LogLevel LogLevel { get; set; } = Logging.LogLevel.Important;
    }
}
