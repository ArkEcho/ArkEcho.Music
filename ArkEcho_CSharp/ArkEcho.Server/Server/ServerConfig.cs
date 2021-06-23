using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public class JsonProperty : Attribute
        {
            public JsonProperty()
            {
            }

        }

        public string FilePath { get; private set; } = string.Empty;

        [JsonProperty]
        public string MusicFolder { get; set; } = string.Empty;

        [JsonProperty]
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
                        foreach (PropertyInfo propInfo in typeof(ServerConfig).GetProperties())
                        {
                            foreach (object attr in propInfo.GetCustomAttributes(true))
                            {
                                JsonProperty authAttr = attr as JsonProperty;
                                if (authAttr != null)
                                {
                                    if (propInfo.PropertyType == typeof(string))
                                        propInfo.SetValue(this, load.ContainsKey(propInfo.Name) ? (string)load[propInfo.Name] : string.Empty);
                                    else if(propInfo.PropertyType == typeof(bool))
                                        propInfo.SetValue(this, load.ContainsKey(propInfo.Name) ? (bool)load[propInfo.Name] : false);
                                }
                            }
                        }

                        // Check if Key exist before Accessing
                        //MusicFolder = load.ContainsKey(JSON_MusicFolder) ? (string)load[JSON_MusicFolder] : string.Empty;
                        //Authorization = load.ContainsKey(JSON_AUTHORIZATION) ? (bool)load[JSON_AUTHORIZATION] : false;

                        foundCorrectExistingFile = true;
                    }
                }
            }

            JObject save = new JObject
            {
                [JSON_MusicFolder] = MusicFolder,
                [JSON_AUTHORIZATION] = Authorization
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
