using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public abstract class RestBase : IRest
    {
        public enum HttpMethods
        {
            Get,
            Post,
        }

        public abstract class HttpResponseBase : IDisposable
        {
            private bool disposedValue;

            public bool Success { get; set; } = false;

            public abstract Task<string> GetResultContentAsStringAsync();
            public abstract Task<byte[]> GetResultContentAsByteArrayAsync();
            public abstract Task CopyContentToStreamAsync(Stream stream);

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing) { }
                    disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
            }
        }

        private bool compression = false;

        public int Timeout { get; set; } = 10000;

        public RestBase(bool compression)
        {
            this.compression = compression;
        }

        public async Task<bool> CheckConnection()
        {
            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, "/api/Control", string.Empty))
                return response != null;
        }

        public async Task<User> AuthenticateUserForLogin(User userToAuthenticate)
        {
            string bodyContent = await userToAuthenticate.SaveToJsonString();
            using (HttpResponseBase response = await makeRequest(HttpMethods.Post, "/api/Authenticate/Login", bodyContent.ToBase64()))
                return await checkAndReturnAuthenticateResult(response);
        }
        public async Task<bool> LogoutUser(Guid guid)
        {
            using (HttpResponseBase response = await makeRequest(HttpMethods.Post, "/api/Authenticate/Logout", guid.ToString()))
                return response != null && response.Success;
        }

        public async Task<User> CheckUserToken(Guid guid)
        {
            using (HttpResponseBase response = await makeRequest(HttpMethods.Post, "/api/Authenticate/Token", guid.ToString()))
                return await checkAndReturnAuthenticateResult(response);
        }

        public async Task<bool> UpdateUser(User userToUpdate)
        {
            string bodyContent = await userToUpdate.SaveToJsonString();
            using (HttpResponseBase response = await makeRequest(HttpMethods.Post, "/api/Authenticate/Update", bodyContent.ToBase64()))
                return response != null && response.Success;
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

        public async Task<Guid> GetMusicLibraryGuid()
        {
            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, $"/api/Music/Library", string.Empty))
            {
                if (response == null || !response.Success)
                    return Guid.Empty;

                if (!Guid.TryParse(await response.GetResultContentAsStringAsync(), out Guid result))
                    return Guid.Empty;
                else
                    return result;
            }
        }

        public async Task<MusicLibrary> GetMusicLibrary()
        {
            MusicLibrary library = new MusicLibrary();

            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, $"/api/Music/Library", string.Empty))
            {
                if (response == null || !response.Success)
                    return null;

                if (!Guid.TryParse(await response.GetResultContentAsStringAsync(), out Guid result))
                    return null;

                library.GUID = result;
            }

            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, "/api/Music/Albums", string.Empty))
            {
                if (response == null || !response.Success)
                    return null;

                library.Album = await Serializer.Deserialize<List<Album>>(await response.GetResultContentAsByteArrayAsync());
            }

            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, "/api/Music/AlbumArtists", string.Empty))
            {
                if (response == null || !response.Success)
                    return null;

                library.AlbumArtists = await Serializer.Deserialize<List<AlbumArtist>>(await response.GetResultContentAsByteArrayAsync());
            }

            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, "/api/Music/Playlists", string.Empty))
            {
                if (response == null || !response.Success)
                    return null;

                library.Playlists = await Serializer.Deserialize<List<Playlist>>(await response.GetResultContentAsByteArrayAsync());
            }

            int count = 0;
            do
            {
                List<MusicFile> files = null;

                using (HttpResponseBase response = await makeRequest(HttpMethods.Get, $"/api/Music/MusicFiles/{count}", string.Empty))
                {
                    if (response == null || !response.Success)
                        break;

                    files = await Serializer.Deserialize<List<MusicFile>>(await response.GetResultContentAsByteArrayAsync());
                }
                count++;

                library.MusicFiles.AddRange(files);

                if (files.Count < Resources.RestMusicFileCount)
                    break;

            } while (true);

            return library;
        }

        public async Task<string> GetAlbumCover(Guid guid)
        {
            using (HttpResponseBase response = await makeRequest(HttpMethods.Get, $"/api/Music/AlbumCover/{guid}", string.Empty))
            {
                if (response == null || !response.Success)
                    return string.Empty;

                return await response.GetResultContentAsStringAsync();
            }
        }

        public async Task<bool> UpdateMusicRating(Guid guid, int rating)
        {
            using (HttpResponseBase response = await makeRequest(HttpMethods.Post, $"/api/Music/Rating/{guid},{rating}", string.Empty))
                return response != null && response.Success;
        }

        public async Task<bool> PostLogging(LogMessage logMessage)
        {
            string bodyContent = await logMessage.SaveToJsonString();

            using (HttpResponseBase response = await makeRequest(HttpMethods.Post, "/api/Logging", bodyContent.ToBase64()))
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
                    using (HttpResponseBase response = await makeRequest(HttpMethods.Get,
                        $"/api/File/ChunkTransfer?file={tfb.GUID}&chunk={chunk.GUID}", string.Empty))
                    {
                        if (response == null || !response.Success)
                        {
                            stream?.Dispose();
                            return null;
                        }

                        stream.Position = chunk.Position;

                        await response.CopyContentToStreamAsync(stream);
                    }
                }

                return stream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on GetFile: {ex.GetFullMessage()}");
                stream?.Dispose();
                return null;
            }
        }

        protected abstract Task<HttpResponseBase> makeRequest(HttpMethods method, string path, string httpContent);

    }
}