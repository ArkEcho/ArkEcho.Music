using System;
using System.IO;
using System.Runtime.Serialization;
using TagLib;

namespace ArkEcho.Core
{
    public class MusicFile
    {
        public Guid ID { get; } = Guid.NewGuid();

        public Guid Album { get; set; } = Guid.Empty;

        public Guid AlbumArtist { get; set; } = Guid.Empty;

        public string Title { get; set; } = string.Empty;

        public string Performer { get; set; } = string.Empty;

        public uint Disc { get; set; } = 0;

        public uint Track { get; set; } = 0;

        public uint Year { get; set; } = 0;

        public string FilePath { get; set; } = string.Empty;

        public MusicFile(string FilePath)
        {
            this.FilePath = FilePath;
        }
    }
}
