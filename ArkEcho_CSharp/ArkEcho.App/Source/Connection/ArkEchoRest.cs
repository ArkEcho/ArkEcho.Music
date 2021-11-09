using RestSharp;
using System.Net;
using System.Threading.Tasks;

namespace ArkEcho.App.Connection
{
    public class ArkEchoRest
    {
        private RestClient client;

        public ArkEchoRest()
        {
            client = new RestClient("https://192.168.178.20:5001/api");
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