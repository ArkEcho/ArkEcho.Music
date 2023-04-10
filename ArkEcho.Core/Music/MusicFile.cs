using System;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MusicFile : TransferFileBase
    {
        [JsonInclude]
        public Guid Album { get; set; } = Guid.Empty;

        [JsonInclude]
        public Guid AlbumArtist { get; set; } = Guid.Empty;

        [JsonInclude]
        public string Title { get; set; } = string.Empty;

        [JsonInclude]
        public string Performer { get; set; } = string.Empty;

        [JsonInclude]
        public int Disc { get; set; } = 0;

        [JsonInclude]
        public int Track { get; set; } = 0;

        [JsonInclude]
        public int Year { get; set; } = 0;

        /// <summary>
        /// Duration of the Musicfile in Milliseconds.
        /// </summary>
        [JsonInclude]
        public int Duration { get; set; } = 0;

        [JsonInclude]
        public int Rating { get; set; } = 0;

        [JsonInclude]
        public int Bitrate { get; set; } = 0;

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
