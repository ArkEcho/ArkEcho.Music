using ArkEcho.Core;
using RestSharp;
using System.Threading;
using System.Threading.Tasks;

namespace ArkEcho.App.Connection
{
    public class ArkEchoRest
    {
        private RestClient client;

        public ArkEchoRest()
        {
#if DEBUG
            client = new RestClient("https://192.168.178.20:5001/api");
#else
            client = new RestClient("https://arkecho.de/api");
#endif
        }

        public async Task<string> GetMusicLibrary()
        {
            RestRequest request = new RestRequest("Music/Library");

            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            var restResponse = await client.ExecuteAsync(request, cancellationTokenSource.Token);

            restResponse.Content = removeLeadingTrailingQuotas(restResponse.Content);

            if (restResponse.IsSuccessful)
                return ZipCompression.UnzipFromBase64(restResponse.Content);
            else
                return string.Empty;
        }

        private string removeLeadingTrailingQuotas(string textWithQuotas)
        {
            string result = textWithQuotas.Remove(0, 1);
            result = result.Remove(result.Length - 1, 1);
            return result;
        }
    }
}