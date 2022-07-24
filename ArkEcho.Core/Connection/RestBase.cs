using System;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public abstract class RestBase : IRestLogging, IRestUser, IRestMusic, IRestFiles
    {
        public enum HttpMethods
        {
            Get,
            Post,
        }

        // TODO: IDisposable
        public abstract class HttpResponseBase
        {
            public bool Success { get; set; } = false;

            public abstract Task<string> GetResultContentAsStringAsync();
            public abstract Task CopyContentToStreamAsync(Stream stream);
        }

        private bool compression = false;

        public int Timeout { get; set; } = 10000;

        public RestBase(bool compression)
        {
            this.compression = compression;
        }

        public bool CheckConnection()
        {
            HttpResponseBase response = makeRequest(HttpMethods.Get, "/api/Control", string.Empty);
            return response != null;
        }

        public async Task<User> AuthenticateUserForLogin(User userToAuthenticate)
        {
            string bodyContent = await userToAuthenticate.SaveToJsonString();
            HttpResponseBase restResponse = makeRequest(HttpMethods.Post, "/api/Authenticate/Login", bodyContent.ToBase64());
            return await checkAndReturnAuthenticateResult(restResponse);
        }

        public async Task<User> CheckUserToken(Guid guid)
        {
            HttpResponseBase restResponse = makeRequest(HttpMethods.Post, "/api/Authenticate/Token", guid.ToString());
            return await checkAndReturnAuthenticateResult(restResponse);
        }

        private async Task<User> checkAndReturnAuthenticateResult(HttpResponseBase response)
        {
            if (response != null && response.Success)
            {
                string content = await response.GetResultContentAsStringAsync();

                content = content.FromBase64();

                if (string.IsNullOrEmpty(content))
                    return null;

                User user = new User();
                await user.LoadFromJsonString(content);
                return user;
            }
            else
                return null;
        }

        public async Task<string> GetMusicLibrary()
        {
            HttpResponseBase response = makeRequest(HttpMethods.Get, "/api/Music", string.Empty);

            if (response != null && response.Success)
            {
                string content = await response.GetResultContentAsStringAsync();

                if (compression)
                    return await ZipCompression.UnzipBase64(content);
                else
                    return content.FromBase64();
            }
            else
                return string.Empty;
        }

        public async Task<string> GetAlbumCover(Guid guid)
        {
            HttpResponseBase response = makeRequest(HttpMethods.Get, $"/api/Music/AlbumCover/{guid}", string.Empty);

            if (response != null && response.Success)
                return await response.GetResultContentAsStringAsync();
            else
                return string.Empty;
        }

        public async Task<bool> PostLogging(LogMessage logMessage)
        {
            string bodyContent = await logMessage.SaveToJsonString();
            HttpResponseBase response = makeRequest(HttpMethods.Post, "/api/Logging", bodyContent.ToBase64());
            return response != null && response.Success;
        }

        public async Task<MemoryStream> GetFile(TransferFileBase tfb)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream(new byte[tfb.FileSize]);

                foreach (TransferFileBase.FileChunk chunk in tfb.Chunks)
                {
                    HttpResponseBase response = makeRequest(HttpMethods.Get,
                        $"/api/File/ChunkTransfer?file={tfb.GUID}&chunk={chunk.GUID}", string.Empty);

                    if (response == null || !response.Success)
                    {
                        stream?.Dispose();
                        return null;
                    }

                    stream.Position = chunk.Position;

                    await response.CopyContentToStreamAsync(stream);
                }

                return stream;
            }
            catch
            {
                stream?.Dispose();
                return null;
            }
        }

        protected abstract HttpResponseBase makeRequest(HttpMethods method, string path, string httpContent);
    }
}