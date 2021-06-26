using ArkEcho.Core;
using System;
using System.IO;

namespace ArkEcho.Server
{
    public class ServerConfig : JsonBase
    {
        public const string FileName = "Config.json";

        public ServerConfig()
        {
        }

        [JsonProperty]
        public string MusicFolder { get; set; } = string.Empty;

        [JsonProperty]
        public bool Authorization { get; private set; } = false;

        [JsonProperty]
        public TestClass TestClass { get; private set; }

        public bool Load(string Folder)
        {
            string filepath = $"{Folder}\\{FileName}";

            Console.WriteLine($"Loading Config File {filepath}");

            string content = string.Empty;
            if (File.Exists(filepath))
                content = File.ReadAllText(filepath);

            bool foundCorrectExistingFile = LoadPropertiesFromJsonString(content);

            // Todo
            //File.WriteAllText(filepath, GetJsonAsString(), System.Text.Encoding.UTF8);

            return foundCorrectExistingFile;
        }

        public void WriteOutputToConsole()
        {
            string prefix = "\t";
            string middle = ": ";

            Console.WriteLine();
            Console.WriteLine("Configuration for ArkEcho.Server:");
            Console.WriteLine($"{prefix}MusicFolder{middle}{MusicFolder}");
            Console.WriteLine($"{prefix}Authorization{middle}{Authorization}");
            Console.WriteLine();
        }
    }
}
