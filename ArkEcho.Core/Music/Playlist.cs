using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Playlist : JsonBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public SortedSet<Guid> MusicFiles { get; set; } = new SortedSet<Guid>();

        public Playlist() : base() { }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
