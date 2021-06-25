using ArkEcho.Core;

namespace ArkEcho.Server
{
    public class TestClass : JsonBase
    {
        [JsonProperty(StandardValue = 123)]
        public int TestInt { get; set; }

        [JsonProperty(StandardValue = 123.45)]
        public double TestDouble { get; set; }
    }
}
