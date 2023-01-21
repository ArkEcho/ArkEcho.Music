using System;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MusicFile : TransferFileBase
    {
        public Guid Album { get; set; } = Guid.Empty;

        public Guid AlbumArtist { get; set; } = Guid.Empty;

        public string Title { get; set; } = string.Empty;

        public string Performer { get; set; } = string.Empty;

        public int Disc { get; set; } = 0;

        public int Track { get; set; } = 0;

        public int Year { get; set; } = 0;

        /// <summary>
        /// Duration of the Musicfile in Milliseconds.
        /// </summary>
        public int Duration { get; set; } = 0;

        /// <summary>
        /// For Serialization and Unit Tests
        /// </summary>
        public MusicFile() : base() { }

        public MusicFile(string filePath) : base(filePath)
        {
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Performer} - {Title}";
            }
        }
    }
}
