using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Playlist : TransferFileBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public string Title { get; set; } = string.Empty;

        [JsonProperty]
        public SortedSet<Guid> MusicFiles { get; set; } = new SortedSet<Guid>();

        /// <summary>
        /// For Serialization and Unit Tests
        /// </summary>
        public Playlist() : base() { }

        public Playlist(string filePath) : base(filePath)
        {
        }

        public long GetDurationInMilliseconds(MusicLibrary library)
        {
            long playlistDuration = 0;

            if (library != null)
                playlistDuration = MusicFiles.Sum(x => library.MusicFiles.Find(y => y.GUID == x).Duration);

            return playlistDuration;
        }

        private string DebuggerDisplay
        {
            get
            {
                return $"{Title}";
            }
        }
    }
}
