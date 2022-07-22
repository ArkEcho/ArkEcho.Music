using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ArkEcho.Core
{
    public class TransferFileBase : JsonBase
    {
        [JsonProperty]
        public string FileName { get; set; }

        [JsonProperty]
        public string FileFormat { get; set; }

        [JsonProperty]
        public long FileSize { get; set; }

        [JsonProperty]
        public string CheckSum { get; set; }

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
            this.FileSize = info.Length;

            createCheckSumAndChunks();
        }

        private void createCheckSumAndChunks()
        {
            CheckSum = getMD5Hash(FullPath);
        }

        public bool TestCheckSum()
        {
            return getMD5Hash(FullPath) == CheckSum;
        }

        public string FullPath
        {
            get
            {
                return $"{Folder.LocalPath}{Resources.FilePathDivider}{FileName}";
            }
        }

        private string getMD5Hash(string filePath)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                MD5 md5 = MD5.Create();
                return Encoding.Default.GetString(md5.ComputeHash(stream)).ToBase64();
            }
        }

    }
}
