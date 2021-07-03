using System;
using System.Collections.Generic;
using System.IO;

namespace ArkEcho.Core
{
    public class SimpleTestClass : JsonBase
    {
        [JsonProperty]
        public int Int { get; set; }
    }
    public class TestClass : JsonBase
    {
        public const string FileName = "TestClass.json";

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
        public List<int> ListInt { get; set; }

        [JsonProperty]
        public int[] ArrayInt { get; set; }

        [JsonProperty]
        public List<SimpleTestClass> ListClass { get; set; }

        [JsonProperty]
        public SimpleTestClass[] ArrayClass { get; set; }

        public TestClass() { }

        public bool Load(string Folder)
        {
            string filepath = $"{Folder}\\{FileName}";

            Console.WriteLine($"Loading Config File {filepath}");

            string content = string.Empty;
            if (File.Exists(filepath))
                content = File.ReadAllText(filepath);

            // Load Props from JSON
            bool foundCorrectExistingFile = LoadPropertiesFromJsonString(content);

            // Write back to add missing Params
            File.WriteAllText(filepath, GetJsonAsString(), System.Text.Encoding.UTF8);

            return foundCorrectExistingFile;
        }
    }
}
