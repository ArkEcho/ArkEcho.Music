using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

namespace ArkEcho.Core
{
    // TODO: Eigene Playlists erstellen speichern verwalten
    // TODO: Playlists ändern und über Rest übertragen
    // TODO: MudBlazor DragZone liste Verschiebbar
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Playlist : TransferFileBase
    {
        [JsonInclude]
        public string Title { get; set; } = string.Empty;

        [JsonInclude]
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
