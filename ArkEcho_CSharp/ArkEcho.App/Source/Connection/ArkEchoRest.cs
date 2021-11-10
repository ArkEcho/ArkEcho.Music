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
            client = new RestClient("https://192.168.178.21:5001/api");
#else
            client = new RestClient("https://arkecho.de/api");
#endif
        }

        public async Task<IRestResponse> GetMusicFileInfo()
        {
            RestRequest request = new RestRequest("Music/MusicFile");

            // execute the request
            IRestResponse response = null;
            response = client.Get(request);
            return response;
        }
    }
}