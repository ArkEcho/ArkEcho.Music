using System;
using System.IO;

namespace ArkEcho.Core
{
    public class TransferFileBase : JsonBase
    {
        [JsonProperty]
        public string FileName { get; set; }

        [JsonProperty]
        public string FileFormat { get; set; }

        public Uri Folder { get; set; }

        /// <summary>
        /// ONLY FOR SERIALIZATION
        /// </summary>
        public TransferFileBase() : base() { }

        public TransferFileBase(string filePath) : base()
        {
            FileInfo info = new FileInfo(filePath);

            this.Folder = new Uri(info.DirectoryName);
            this.FileName = info.Name;
            this.FileFormat = info.Extension.Substring(1);
        }

        public string FullPath
        {
            get
            {
                return $"{Folder.LocalPath}{Resources.FilePathDivider}{FileName}";
            }
        }
    }
}
