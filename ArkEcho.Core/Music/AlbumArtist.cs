using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class AlbumArtist
    {
        [JsonInclude]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonInclude]
        public List<Guid> AlbumID { get; set; } = new List<Guid>();

        [JsonInclude]
        public List<Guid> MusicFileIDs { get; set; } = new List<Guid>();

        [JsonInclude]
        public string Name { get; set; } = string.Empty;

        public AlbumArtist() { }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
