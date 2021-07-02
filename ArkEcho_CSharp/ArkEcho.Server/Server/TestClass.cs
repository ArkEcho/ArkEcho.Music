using ArkEcho.Core;
using System.Collections.Generic;

namespace ArkEcho.Server
{
    public class TestClass : JsonBase
    {
        [JsonProperty]
        public int TestInt { get; set; } = 123;

        [JsonProperty]
        public double TestDouble { get; set; } = 123.45;

        [JsonProperty]
        public List<int> TestList { get; set; }

        [JsonProperty]
        public int[] TestArray { get; set; }
    }
}
