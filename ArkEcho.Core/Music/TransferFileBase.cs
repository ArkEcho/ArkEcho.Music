using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ArkEcho.Core
{
    public class TransferFileBase : JsonBase
    {
        public class FileChunk : JsonBase
        {
            [JsonProperty]
            public Guid GUID { get; set; } = Guid.NewGuid();

            [JsonProperty]
            public int Size { get; set; } = 0;

            [JsonProperty]
            public int Position { get; set; } = 0;
        }

        [JsonProperty]
        public Guid GUID { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public string FileName { get; set; }

        [JsonProperty]
        public string FileFormat { get; set; }

        [JsonProperty]
        public long FileSize { get; set; }

        [JsonProperty]
        public string CheckSum { get; set; }

        [JsonProperty]
        public List<FileChunk> Chunks { get; set; }

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

            if (info.Length > Resources.MaxFileSize)
                throw new Exception($"File is too large, {filePath}!");

            this.FileSize = info.Length;

            createCheckSumAndChunks();
        }

        public bool TestCheckSum()
        {
            return GetCheckSum(FullPath) == CheckSum;
        }

        public string FullPath
        {
            get
            {
                return $"{Folder.LocalPath}{Resources.FilePathDivider}{FileName}";
            }
        }

        private void createCheckSumAndChunks()
        {
            CheckSum = GetCheckSum(FullPath);

            Chunks = new List<FileChunk>();

            int position = 0;
            int sizeToChunk = (int)FileSize;

            do
            {
                FileChunk chunk = new FileChunk();
                chunk.Position = position;
                chunk.Size = sizeToChunk > Resources.RestChunkSize ? Resources.RestChunkSize : sizeToChunk;
                Chunks.Add(chunk);

                sizeToChunk -= chunk.Size;
                position += chunk.Size;
            }
            while (sizeToChunk > 0);
        }

        public static string GetCheckSum(string filePath)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] data = sha256Hash.ComputeHash(File.ReadAllBytes(filePath));

                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));

                return sBuilder.ToString();
            }
        }
    }
}
