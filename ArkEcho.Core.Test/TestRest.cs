using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core.Test
{
    public class TestRest : RestBase
    {
        List<TransferFileBase> files = null;

        public class HttpResponseTest : HttpResponseBase
        {
            byte[] data = null;

            public HttpResponseTest(bool success, byte[] data)
            {
                Success = success;
                this.data = data;
            }

            public override Task CopyContentToStreamAsync(Stream stream)
            {
                return stream.WriteAsync(data, 0, data.Length);
            }

            public override Task<byte[]> GetResultByteArrayAsync()
            {
                throw new NotImplementedException();
            }

            public override Task<Guid> GetResultGuidAsync()
            {
                throw new NotImplementedException();
            }

            public override Task<string> GetResultStringAsync()
            {
                throw new NotImplementedException();
            }
        }

        public TestRest(List<TransferFileBase> files) : base()
        {
            this.files = files;
        }

        protected override async Task<HttpResponseBase> makeRequest(HttpMethods method, string path, string httpContent)
        {
            if (method != HttpMethods.Get)
                return null;

            if (path.StartsWith("/api/File/ChunkTransfer?"))
            {
                return await ChunkTransfer(path);
            }
            else
                return null;
        }

        private async Task<HttpResponseBase> ChunkTransfer(string path)
        {
            int guidLength = Guid.NewGuid().ToString().Length;
            Guid fileGuid = Guid.Parse(path.Substring(path.IndexOf("&chunk") - guidLength, guidLength));
            Guid chunkGuid = Guid.Parse(path.Substring(path.Length - guidLength));

            TransferFileBase tfb = files.Find(x => x.GUID == fileGuid);
            if (tfb == null)
                return null;

            TransferFileBase.FileChunk chunk = tfb.Chunks.Find(x => x.GUID == chunkGuid);
            if (chunk == null)
                return null;

            byte[] data = new byte[chunk.Size];
            using (FileStream fs = new FileStream(tfb.FullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                fs.Position = chunk.Position;
                fs.Read(data, 0, chunk.Size);
            }

            return new HttpResponseTest(true, data);
        }
    }
}
