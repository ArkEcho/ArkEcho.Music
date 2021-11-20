using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class AlbumArtist : JsonBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public List<Guid> AlbumID { get; set; } = new List<Guid>();

        [JsonProperty]
        public List<Guid> MusicFileIDs { get; set; } = new List<Guid>();

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        public AlbumArtist() : base() { }
    }
}
