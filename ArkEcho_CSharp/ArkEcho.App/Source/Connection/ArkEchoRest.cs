using Java.IO;
using RestSharp;

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

            // execute the request
            IRestResponse response = null;
            response = client.Get(request);

            if (response.IsSuccessful)
                return response.Content;
            else
                return string.Empty;
        }
    }
}