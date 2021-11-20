using RestSharp;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArkEcho.App.Connection
{
    public class ArkEchoRest
    {
        private RestClient client;

        public ArkEchoRest(string connectionUrl)
        {
            client = new RestClient(connectionUrl);
        }

        public async Task<string> GetMusicLibrary()
        {
            var restResponse = await makeRestCall("Music");

            if (restResponse.IsSuccessful)
            {
                // TODO: Compression zu abschaltbar
                restResponse.Content = removeLeadingTrailingQuotas(restResponse.Content);
                return Encoding.UTF8.GetString(Convert.FromBase64String(restResponse.Content));//ZipCompression.UnzipFromBase64(restResponse.Content);
            }
            else
                return string.Empty;
        }

        public async Task<byte[]> GetMusicFile(Guid guid)
        {
            var restResponse = await makeRestCall($"Music/{guid}");

            if (restResponse.IsSuccessful)
            {
                return restResponse.RawBytes;// ZipCompression.UnzipByteArray(restResponse.RawBytes);
            }
            else
                return null;
        }

        private async Task<IRestResponse> makeRestCall(string path)
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