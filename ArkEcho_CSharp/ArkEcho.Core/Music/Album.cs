using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class Album : JsonBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public List<Guid> MusicFiles { get; set; } = new List<Guid>();

        [JsonProperty]
        public Guid AlbumArtist { get; set; } = Guid.Empty;

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public uint TrackCount { get; set; } = 0;

        [JsonProperty]
        public uint DiscCount { get; set; } = 0;

        [JsonProperty]
        public uint Year { get; set; } = 0;

        public Album() : base() { }
    }
}
