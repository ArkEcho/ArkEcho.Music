using System;
using System.Collections.Generic;
using System.Text;

namespace ArkEcho.Core
{
    public class Album
    {
        public Album()
        {

        }

        public Guid ID { get; } = Guid.NewGuid();

        public List<Guid> MusicFiles { get; set; } = new List<Guid>();

        public Guid AlbumArtist { get; set; } = Guid.Empty;

        public string Name { get; set; } = string.Empty;

        public uint TrackCount { get; set; } = 0;

        public uint DiscCount { get; set; } = 0;

        public uint Year { get; set; } = 0;
    }
}
