using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class Rest : RestBase
    {
        private HttpClient client = null;

        public class HttpResponse : HttpResponseBase
        {
            HttpResponseMessage responseMessage = null;

            public HttpResponse(HttpResponseMessage responseMessage)
            {
                this.responseMessage = responseMessage;
                Success = responseMessage.IsSuccessStatusCode;
            }

            public override Task CopyContentToStreamAsync(Stream stream)
            {
                return responseMessage.Content.CopyToAsync(stream);
            }

            public override Task<string> GetResultStringAsync()
            {
                return responseMessage.Content.ReadAsStringAsync();
            }

            public override Task<byte[]> GetResultByteArrayAsync()
            {
                return responseMessage.Content.ReadAsByteArrayAsync();
            }

            protected override void Dispose(bool disposing)
            {
                responseMessage?.Dispose();
                base.Dispose(disposing);
            }

            public override async Task<Guid> GetResultGuidAsync()
            {
                string result = await responseMessage.Content.ReadAsStringAsync();
                if (Guid.TryParse(result, out Guid guid))
                    return guid;
                else
                    return Guid.Empty;
            }
        }

        public Rest(string connectionUrl, bool userHttpClientHandler) : base()
        {
            if (userHttpClientHandler)
            {
                // TODO: Disable on Release Build?
                HttpClientHandler handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (request, cert, chain, errors) => true;
                client = new HttpClient(handler) { BaseAddress = new Uri(connectionUrl), Timeout = new TimeSpan(0, 0, 10) };
            }
            else
                client = new HttpClient() { BaseAddress = new Uri(connectionUrl), Timeout = new TimeSpan(0, 0, 10) };
        }

        protected override async Task<HttpResponseBase> makeRequest(HttpMethods method, string path, string httpContent, bool useApiToken)
        {
            HttpMethod httpMethod = null;
            switch (method)
            {
                case HttpMethods.Get:
                    httpMethod = HttpMethod.Get;
                    break;
                case HttpMethods.Post:
                    httpMethod = HttpMethod.Post;
                    break;
                default:
                    throw new Exception($"Not Supported HTTP Method {method}!");
            }

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, path);
            if (!string.IsNullOrEmpty(httpContent))
                request.Content = new StringContent(httpContent);

            if (useApiToken)
                request.Headers.Add(Resources.ApiTokenHeaderKey, ApiToken.ToString());

            HttpResponse response = null;

            try
            {
                HttpResponseMessage responseNet = await client.SendAsync(request);
                response = new HttpResponse(responseNet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception making Rest call: {ex.Message}");
            }

            return response;
        }
    }
}