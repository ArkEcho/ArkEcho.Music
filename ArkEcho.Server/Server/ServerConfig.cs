using ArkEcho.Core;

namespace ArkEcho.Server
{
    public class ServerConfig : JsonBase
    {
        public ServerConfig(string FileName) : base(FileName) { }

        [JsonProperty]
        public string MusicFolder { get; private set; } = string.Empty;

        [JsonProperty]
        public bool Authorization { get; private set; } = false;

        [JsonProperty]
        public bool Compression { get; private set; } = false;

        [JsonProperty]
        public int Port { get; private set; } = 5001;
    }
}
