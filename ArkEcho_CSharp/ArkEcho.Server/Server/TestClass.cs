using ArkEcho.Core;
using System;
using System.Collections.Generic;

namespace ArkEcho.Server
{
    public class TestClass : JsonBase
    {
        [JsonProperty(StandardValue = 123)]
        public int TestInt { get; set; }

        [JsonProperty(StandardValue = 123.45)]
        public double TestDouble { get; set; }

        [JsonProperty]
        public List<Guid> TestList { get; set; }

        [JsonProperty]
        public int[] TestArray { get; set; }
    }
}
