using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArkEcho.Core;
using RestSharp;

namespace ArkEcho.App.Connection
{
    public class ArkEchoRest
    {
        private RestClient client;

        public ArkEchoRest()
        {
            client = new RestClient("https://192.168.0.65:5001/api");
        }

        public async Task<IRestResponse> getMusic()
        {
            RestRequest request = new RestRequest("/MusicFiles", Method.GET);

            // execute the request
            IRestResponse response = null;
            await Task.Run(()=> response = client.Execute(request));
            return response;
        }
    }
}