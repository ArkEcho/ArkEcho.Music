using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Playlist : TransferFileBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public string Name { get; set; } = string.Empty;

        [JsonProperty]
        public SortedSet<Guid> MusicFiles { get; set; } = new SortedSet<Guid>();

        /// <summary>
        /// ONLY FOR SERIALIZATION
        /// </summary>
        public Playlist() : base() { }

        public Playlist(string filePath) : base(filePath)
        {
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
