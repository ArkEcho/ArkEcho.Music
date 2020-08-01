using System;
using RestSharp;

namespace ArkEcho.App.Connection
{
    public class ArkEchoRest
    {
        private const string WEATHERFORECAST = "WeatherForecast";

        private RestClient client;

        public ArkEchoRest()
        {
            client = new RestClient("https://192.168.0.65:5001");
        }

        public string getWeather()
        {
            RestRequest request = new RestRequest("/WeatherForecast", Method.GET);

            // execute the request
            IRestResponse response = client.Execute(request);
            return response.Content;
        }

        public string getUserByID(int ID)
        {
            RestRequest request = new RestRequest(WEATHERFORECAST + "/{id}", Method.GET);
            request.AddUrlSegment("id", ID.ToString()); // replaces matching token in request.Resource

            // execute the request
            IRestResponse response = client.Execute(request);
            return response.Content;
        }
    }
}