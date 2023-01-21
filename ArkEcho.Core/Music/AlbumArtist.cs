using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class AlbumArtist
    {
        public Guid GUID { get; set; } = Guid.NewGuid();

        public List<Guid> AlbumID { get; set; } = new List<Guid>();

        public List<Guid> MusicFileIDs { get; set; } = new List<Guid>();

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
