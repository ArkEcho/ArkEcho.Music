using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class Rest
    {
        private HttpClient client = null;
        private bool compression = false;

        public int Timeout { get; set; } = 10000;

        public Rest(string connectionUrl, bool compression)
        {
            // TODO: Disable on Release Build
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;

            client = new HttpClient(handler) { BaseAddress = new Uri(connectionUrl), Timeout = new TimeSpan(0, 0, 30) };

            this.compression = compression;
        }

        public bool CheckConnection()
        {
            HttpResponseMessage response = makeRequest(HttpMethod.Get, "/api/Control", string.Empty);
            return response != null;
        }

        public async Task<User> AuthenticateUserForLogin(User userToAuthenticate)
        {
            string bodyContent = await userToAuthenticate.SaveToJsonString();
            HttpResponseMessage restResponse = makeRequest(HttpMethod.Post, "/api/Authenticate/Login", bodyContent.ToBase64());
            return await checkAndReturnAuthenticateResult(restResponse);
        }

        public async Task<User> CheckUserToken(Guid guid)
        {
            HttpResponseMessage restResponse = makeRequest(HttpMethod.Post, "/api/Authenticate/Token", guid.ToString());
            return await checkAndReturnAuthenticateResult(restResponse);
        }

        private async Task<User> checkAndReturnAuthenticateResult(HttpResponseMessage response)
        {
            if (response != null && response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

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
            HttpResponseMessage response = makeRequest(HttpMethod.Get, "/api/Music", string.Empty);

            if (response != null && response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

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
            HttpResponseMessage response = makeRequest(HttpMethod.Get, $"/api/Music/AlbumCover/{guid}", string.Empty);

            if (response != null && response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                return string.Empty;
        }

        public async Task<byte[]> GetMusicFile(Guid guid)
        {
            HttpResponseMessage response = makeRequest(HttpMethod.Get, $"/api/Music/{guid}", string.Empty);

            if (response != null && response.IsSuccessStatusCode)
            {
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                if (compression)
                    return await ZipCompression.Unzip(content);
                else
                    return content;
            }
            else
                return new byte[0];
        }

        public async Task<bool> PostLogging(LogMessage logMessage)
        {
            string bodyContent = await logMessage.SaveToJsonString();
            HttpResponseMessage response = makeRequest(HttpMethod.Post, "/api/Logging", bodyContent.ToBase64());
            return response != null && response.IsSuccessStatusCode;
        }

        private HttpResponseMessage makeRequest(HttpMethod method, string path, string httpContent)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, path);
            if (!string.IsNullOrEmpty(httpContent))
                request.Content = new StringContent(httpContent);

            HttpResponseMessage response = null;
            Task.Factory.StartNew(() =>
            {
                try
                {
                    response = client.SendAsync(request).Result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception making Rest call: {ex.Message}");
                }
            }
            ).Wait(Timeout);

            return response;
        }
    }
}