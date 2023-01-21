using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Album
    {
        public Guid GUID { get; set; } = Guid.NewGuid();

        public List<Guid> MusicFiles { get; set; } = new List<Guid>();

        public Guid AlbumArtist { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public int TrackCount { get; set; } = 0;

        public int DiscCount { get; set; } = 0;

        public int Year { get; set; } = 0;

        public string Cover64 { get; set; } = null;

        public Album() : base() { }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Name}";
            }
        }
    }
}
