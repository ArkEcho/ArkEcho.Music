using System;
using System.Collections.Generic;

namespace ArkEcho.Core
{
    public class AlbumArtist : JsonBase
    {
        [JsonProperty]
        public Guid GUID { get; } = Guid.NewGuid();

        [JsonProperty]
        public List<Guid> AlbumID { get; private set; } = new List<Guid>();

        [JsonProperty]
        public List<Guid> MusicFileIDs { get; private set; } = new List<Guid>();

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        public AlbumArtist() : base() { }
    }
}
