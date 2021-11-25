using ArkEcho.Core;
using RestSharp;
using System;
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
            client.Timeout = 5000;
        }

        public async Task<string> GetMusicLibrary()
        {
            var restResponse = await makeRestCall("Music");

            if (restResponse.IsSuccessful)
            {
                restResponse.Content = removeLeadingTrailingQuotas(restResponse.Content);

                if (AppModel.Instance.Config.Compression)
                    return await ZipCompression.UnzipBase64(restResponse.Content);
                else
                    return restResponse.Content.FromBase64().GetString();
            }
            else
                return string.Empty;
        }

        public async Task<byte[]> GetMusicFile(Guid guid)
        {
            var restResponse = await makeRestCall($"Music/{guid}");

            if (restResponse.IsSuccessful)
            {
                if (AppModel.Instance.Config.Compression)
                    return await ZipCompression.Unzip(restResponse.RawBytes);
                else
                    return restResponse.RawBytes;
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