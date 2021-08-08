using System;
using System.IO;

namespace ArkEcho.Core
{
    public class MusicFile : JsonBase
    {
        [JsonProperty]
        public Guid GUID { get; } = Guid.NewGuid();

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
        public string RemoteFolder { get; } = string.Empty;

        [JsonProperty]
        public string RemoteFileName { get; } = string.Empty;

        [JsonProperty]
        public string FileFormat { get; } = string.Empty;

        public string LocalFileName { get; set; } = string.Empty;

        public MusicFile(string FilePath)
        {
            FileInfo info = new FileInfo(FilePath);

            this.RemoteFolder = info.DirectoryName;
            this.RemoteFileName = info.Name;

            string extensionCleared = info.Extension.Substring(1);
            if (Resources.SupportedFileFormats.Contains(extensionCleared))
                FileFormat = extensionCleared;
            else
                FileFormat = "ERROR";
        }

        public string GetFullFilePath()
        {
            return $"{RemoteFolder}\\{RemoteFileName}";
        }
    }
}
