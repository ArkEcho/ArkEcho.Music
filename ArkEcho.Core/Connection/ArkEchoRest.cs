using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class ArkEchoRest
    {
        private HttpClient client = null;
        private bool compression = false;

        public ArkEchoRest(string connectionUrl, bool compression)
        {
            client = new HttpClient() { BaseAddress = new Uri(connectionUrl), Timeout = new TimeSpan(0, 0, 30) };

            this.compression = compression;
        }

        public async Task<User> AuthenticateUserForLogin(User userToAuthenticate)
        {
            string bodyContent = await userToAuthenticate.SaveToJsonString();
            HttpResponseMessage restResponse = await postRequest("/api/Authenticate/Login", bodyContent.ToBase64());

            return await checkAndReturnAuthenticateResult(restResponse);
        }

        public async Task<User> CheckUserToken(Guid guid)
        {
            HttpResponseMessage restResponse = await postRequest("/api/Authenticate/Token", guid.ToString());

            return await checkAndReturnAuthenticateResult(restResponse);
        }

        private async Task<User> checkAndReturnAuthenticateResult(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
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
            HttpResponseMessage response = await getRequest("/api/Music");

            if (response.IsSuccessStatusCode)
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

        public async Task<byte[]> GetMusicFile(Guid guid)
        {
            HttpResponseMessage response = await getRequest($"/api/Music/{guid}");

            if (response.IsSuccessStatusCode)
            {
                byte[] content = await response.Content.ReadAsByteArrayAsync();
                if (compression)
                    return await ZipCompression.Unzip(content);
                else
                    return content;
            }
            else
                return null;
        }

        private async Task<HttpResponseMessage> getRequest(string path)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, path);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            return await client.SendAsync(request, cancellationTokenSource.Token);
        }

        private async Task<HttpResponseMessage> postRequest(string path, string httpContent)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, path);
            request.Content = new StringContent(httpContent);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            return await client.SendAsync(request, cancellationTokenSource.Token);
        }

        //private string removeLeadingTrailingQuotas(string textWithQuotas)
        //{
        //    string result = textWithQuotas.Remove(0, 1);
        //    result = result.Remove(result.Length - 1, 1);
        //    return result;
        //}
    }
}