using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Album
    {
        [JsonInclude]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonInclude]
        public List<Guid> MusicFiles { get; set; } = new List<Guid>();

        [JsonInclude]
        public Guid AlbumArtist { get; set; } = Guid.Empty;

        [JsonInclude]
        public string Name { get; set; } = string.Empty;

        [JsonInclude]
        public int TrackCount { get; set; } = 0;

        [JsonInclude]
        public int DiscCount { get; set; } = 0;

        [JsonInclude]
        public int Year { get; set; } = 0;

        [JsonIgnore]
        public string Cover64 { get; set; } = null;

        public Album() { }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
