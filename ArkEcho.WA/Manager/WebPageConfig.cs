using ArkEcho.RazorPage.Data;

namespace ArkEcho.WebPage
{
    public class WebPageConfig : RazorConfig
    {
        public WebPageConfig(string fileName) : base(fileName) { }

        [JsonProperty]
        public int Port { get; set; } = 5001;
    }
}
