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

        public ServerConfig(string Folder)
        {
            this.FilePath = $"{Folder}\\{FileName}";
        }

        public readonly List<Type> SupportedTypes = new List<Type>() { typeof(string), typeof(bool), typeof(int) };
        public class JsonProperty : Attribute
        {
            public object StandardValue { get; set; } = string.Empty;
        }

        public string FilePath { get; private set; } = string.Empty;

        [JsonProperty]
        public string MusicFolder { get; set; }

        [JsonProperty(StandardValue = false)]
        public bool Authorization { get; private set; }

        public bool Load()
        {
            Console.WriteLine($"Loading Config File {FilePath}");
            bool foundCorrectExistingFile = false;
            JObject data = null;

            if (File.Exists(FilePath))
            {
                string content = File.ReadAllText(FilePath);

                if (!string.IsNullOrEmpty(content))
                {
                    // Handling for File path in JSON -.-
                    content=content.Replace("\\", "\\\\");
                    try
                    {
                        data = JObject.Parse(content);
                    }
                    catch(Exception)
                    {
                        Console.WriteLine($"### Exception parsing Config File!");
                    }
                }
            }

            if (data != null)
                foundCorrectExistingFile = true;
            else
                data = new JObject();

            foreach (PropertyInfo propInfo in typeof(ServerConfig).GetProperties())
            {
                foreach (object attr in propInfo.GetCustomAttributes(true))
                {
                    JsonProperty authAttr = attr as JsonProperty;
                    if (authAttr != null)
                    {
                        if (propInfo.PropertyType == typeof(string))
                        {
                            if (data.ContainsKey(propInfo.Name))
                                propInfo.SetValue(this, (string)data[propInfo.Name]);
                            else
                            {
                                propInfo.SetValue(this, authAttr.StandardValue);
                                data[propInfo.Name] = (string)authAttr.StandardValue;
                            }
                        }
                        else if (propInfo.PropertyType == typeof(bool))
                        {
                            if (data.ContainsKey(propInfo.Name))
                                propInfo.SetValue(this, (bool)data[propInfo.Name]);
                            else
                            {
                                propInfo.SetValue(this, authAttr.StandardValue);
                                data[propInfo.Name] = (bool)authAttr.StandardValue;
                            }
                        }
                    }
                }
            }

            string saveContent = data.ToString();
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
