using RestSharp;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class ArkEchoRest
    {
        private RestClient client = null;
        private bool compression = false;

        public ArkEchoRest(string connectionUrl, bool compression)
        {
            client = new RestClient(connectionUrl);

            this.compression = compression;
        }

        public async Task<string> GetMusicLibrary()
        {
            var restResponse = await makeRestCall("Music");

            if (restResponse.IsSuccessful)
            {
                string content = removeLeadingTrailingQuotas(restResponse.Content);

                if (compression)
                    return await ZipCompression.UnzipBase64(content);
                else
                    return content.FromBase64().GetString();
            }
            else
                return string.Empty;
        }

        public async Task<byte[]> GetMusicFile(Guid guid)
        {
            var restResponse = await makeRestCall($"Music/{guid}");

            if (restResponse.IsSuccessful)
            {
                if (compression)
                    return await ZipCompression.Unzip(restResponse.RawBytes);
                else
                    return restResponse.RawBytes;
            }
            else
                return null;
        }

        private async Task<RestResponse> makeRestCall(string path)
        {
            RestRequest request = new RestRequest(path);

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            return await client.ExecuteAsync(request, cancellationTokenSource.Token);
        }

        private string removeLeadingTrailingQuotas(string textWithQuotas)
        {
            string result = textWithQuotas.Remove(0, 1);
            result = result.Remove(result.Length - 1, 1);
            return result;
        }
    }
}