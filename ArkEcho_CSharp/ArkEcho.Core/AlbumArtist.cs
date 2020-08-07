using System;
using System.Collections.Generic;
using System.Text;

namespace ArkEcho.Core
{
    public class AlbumArtist
    {
        public AlbumArtist()
        {

        }

        public Guid ID { get; } = Guid.NewGuid();

        public List<Guid> AlbumID { get; set; } = new List<Guid>();

        public List<Guid> MusicFileIDs { get; set; } = new List<Guid>();

        public string Name { get; set; } = string.Empty;
    }
}
