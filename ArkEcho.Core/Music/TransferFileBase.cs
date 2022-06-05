using System;
using System.IO;

namespace ArkEcho.Core
{
    public class TransferFileBase : JsonBase
    {
        [JsonProperty]
        public string FileName { get; private set; }

        [JsonProperty]
        public string FileFormat { get; private set; }

        public Uri Folder { get; private set; }

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

        public string GetFullPathWindows()
        {
            return $"{Folder.LocalPath}\\{FileName}";
        }

        public string GetFullPathAndroid()
        {
            return $"{Folder.LocalPath}/{FileName}";
        }
    }
}
