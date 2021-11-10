using ArkEcho.Core;
using System;

namespace ArkEcho.Server
{
    public class ServerConfig : JsonBase
    {
        public ServerConfig(string FileName) : base(FileName) { }

        [JsonProperty]
        public string MusicFolder { get; private set; } = string.Empty;

        [JsonProperty]
        public bool Authorization { get; private set; } = false;

        [JsonProperty]
        public int Port { get; private set; } = 5001;

        public void WriteOutputToConsole()
        {
            // TODO: In Basis Klasse -> Richtige Reihenfolge Reflection alle etc.
            string prefix = "\t";
            string middle = ": ";

            Console.WriteLine();
            Console.WriteLine("Configuration for ArkEcho.Server:");
            Console.WriteLine($"{prefix}MusicFolder{middle}{MusicFolder}");
            Console.WriteLine($"{prefix}Authorization{middle}{Authorization}");
            Console.WriteLine($"{prefix}Port{middle}{Port}");
            Console.WriteLine();
        }
    }
}
