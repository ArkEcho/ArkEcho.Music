using System;
using System.Diagnostics;
using System.IO;

namespace ArkEcho.Core
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class MusicFile : JsonBase
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

        [JsonProperty]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty]
        public string FileFormat { get; set; } = string.Empty;

        public Uri Folder { get; set; } = null;

        /// <summary>
        /// ONLY FOR SERIALIZATION
        /// </summary>
        public MusicFile() : base() { }

        public MusicFile(string FilePath) : base()
        {
            FileInfo info = new FileInfo(FilePath);

            this.Folder = new Uri(info.DirectoryName);
            this.FileName = info.Name;
            this.FileFormat = info.Extension.Substring(1);
        }

        public string GetFullPathWindows()
        {
            return $"{Folder.LocalPath}\\{FileName}";
        }

        public string GetFullPathAndroid()
        {
            return $"{Folder.LocalPath}/{FileName}";
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
