using System;
using System.IO;

namespace ArkEcho.Core
{
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

        public string Folder { get; set; } = string.Empty;

        [JsonProperty]
        public string FileName { get; set; } = string.Empty;

        [JsonProperty]
        public string FileFormat { get; set; } = string.Empty;

        /// <summary>
        /// ONLY FOR SERIALIZATION
        /// </summary>
        public MusicFile() : base() { }

        public MusicFile(string FilePath) : base()
        {
            FileInfo info = new FileInfo(FilePath);

            this.Folder = PathHandling.ReplaceBackForwardSlashPath(info.DirectoryName);

            this.FileName = info.Name;

            string extensionCleared = info.Extension.Substring(1);
            if (Resources.SupportedFileFormats.Contains(extensionCleared))
                FileFormat = extensionCleared;
            else
                FileFormat = "ERROR";
        }

        public string GetFullFilePath()
        {
            return $"{Folder}/{FileName}";
        }
    }
}
