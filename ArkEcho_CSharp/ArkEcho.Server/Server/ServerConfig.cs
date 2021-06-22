using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public class ServerConfig
    {
        public const string FileName = "Config.json";

        private const string JSON_MusicFolder = "MusicFolder";
        private const string JSON_AUTHORIZATION = "Authorization";

        public ServerConfig(string Folder)
        {
            this.FilePath = $"{Folder}\\{FileName}";
        }

        public string FilePath { get; private set; } = string.Empty;

        public string MusicFolder { get; private set; } = string.Empty;

        public bool Authorization { get; private set; } = false;

        public bool Load()
        {
            Console.WriteLine($"Loading Config File {FilePath}");
            bool foundCorrectExistingFile = false;

            if (File.Exists(FilePath))
            {
                string content = File.ReadAllText(FilePath);

                if (!string.IsNullOrEmpty(content))
                {
                    // Handling for File path in JSON -.-
                    content=content.Replace("\\", "\\\\");
                    JObject load = JObject.Parse(content);

                    if (load != null)
                    {
                        // Check if Key exist before Accessing
                        MusicFolder = load.ContainsKey(JSON_MusicFolder) ? load[JSON_MusicFolder].ToString() : string.Empty;
                        Authorization = load.ContainsKey(JSON_AUTHORIZATION) ? load[JSON_AUTHORIZATION].ToString().Equals(true.ToString(), StringComparison.OrdinalIgnoreCase) : false;

                        foundCorrectExistingFile = true;
                    }
                }
            }

            JObject save = new JObject
            {
                [JSON_MusicFolder] = MusicFolder,
                [JSON_AUTHORIZATION] = Authorization.ToString()
            };

            string saveContent = save.ToString();
            saveContent = saveContent.Replace("\\\\", "\\");
            File.WriteAllText(FilePath, saveContent, System.Text.Encoding.UTF8);

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
