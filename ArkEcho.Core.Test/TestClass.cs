using System;
using System.Collections.Generic;

namespace ArkEcho.Core.Test
{
    public class SimpleTestClass : JsonBase
    {
        [JsonProperty]
        public int Int { get; set; }
    }
    public class TestClass : JsonBase
    {
        [JsonProperty]
        public bool Boolean { get; set; }

        [JsonProperty]
        public int Int { get; set; }

        [JsonProperty]
        public double Double { get; set; }

        [JsonProperty]
        public string String { get; set; }

        [JsonProperty]
        public Guid Guid { get; set; }

        [JsonProperty]
        public DateTime DateTime { get; set; }

        [JsonProperty]
        public Uri Uri { get; set; }

        [JsonProperty]
        public List<int> ListInt { get; set; }

        [JsonProperty]
        public int[] ArrayInt { get; set; }

        [JsonProperty]
        public List<SimpleTestClass> ListClass { get; set; }

        [JsonProperty]
        public SimpleTestClass[] ArrayClass { get; set; }

        public TestClass() { }
    }
}
