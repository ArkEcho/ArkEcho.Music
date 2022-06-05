﻿using System;
using System.Diagnostics;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MusicFile : TransferFileBase
    {
        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public Guid Album { get; set; } = Guid.Empty;

        [JsonProperty]
        public Guid AlbumArtist { get; set; } = Guid.Empty;

        [JsonProperty]
        public string Title { get; set; } = string.Empty;

        [JsonProperty]
        public string Performer { get; set; } = string.Empty;

        [JsonProperty]
        public uint Disc { get; set; } = 0;

        [JsonProperty]
        public uint Track { get; set; } = 0;

        [JsonProperty]
        public uint Year { get; set; } = 0;

        [JsonProperty]
        public long Duration { get; set; } = 0;

        /// <summary>
        /// ONLY FOR SERIALIZATION
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
