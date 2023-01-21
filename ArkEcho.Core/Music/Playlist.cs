using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Playlist : TransferFileBase
    {
        public string Title { get; set; } = string.Empty;

        public List<Guid> MusicFiles { get; set; } = new List<Guid>();

        public Playlist()
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
                return $"{Title}, {MusicFiles.Count} Files";
            }
        }
    }
}
