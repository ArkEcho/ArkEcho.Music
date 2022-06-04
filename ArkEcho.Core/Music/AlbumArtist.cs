using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
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

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
