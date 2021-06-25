using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Reflection;

namespace ArkEcho.Server
{
    public class ServerConfig
    {
        public const string FileName = "Config.json";

        public ServerConfig(string Folder)
        {
            this.FilePath = $"{Folder}\\{FileName}";
        }

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
                    content = content.Replace("\\", "\\\\");
                    try
                    {
                        data = JObject.Parse(content);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"### Exception parsing Config File!");
                    }
                }
            }

            if (data != null)
                foundCorrectExistingFile = true;
            else
                data = new JObject();

            foreach (PropertyInfo info in typeof(ServerConfig).GetProperties())
            {
                foreach (object attr in info.GetCustomAttributes(true))
                {
                    JsonProperty attribute = (JsonProperty)attr;
                    if (attribute != null)
                    {
                        if (info.PropertyType == typeof(string))
                            checkKeyAndSetProperty<string>(attribute, data, info);
                        else if (info.PropertyType == typeof(bool))
                            checkKeyAndSetProperty<bool>(attribute, data, info);
                        else if (info.PropertyType == typeof(Guid))
                            checkKeyAndSetProperty<Guid>(attribute, data, info);
                        else if (info.PropertyType == typeof(int))
                            checkKeyAndSetProperty<int>(attribute, data, info);
                        else if (info.PropertyType == typeof(double))
                            checkKeyAndSetProperty<double>(attribute, data, info);
                    }
                }
            }

            string saveContent = data.ToString();
            saveContent = saveContent.Replace("\\\\", "\\");
            File.WriteAllText(FilePath, saveContent, System.Text.Encoding.UTF8);

            return foundCorrectExistingFile;
        }

        public void checkKeyAndSetProperty<T>(JsonProperty attribute, JObject data, PropertyInfo info)
        {
            if (!data.ContainsKey(info.Name)) // If key doesnt exist, set Standardvalue
                data[info.Name] = (dynamic)(T)attribute.StandardValue;
            info.SetValue(this, Convert.ChangeType(data[info.Name], typeof(T)));

            //if (!data.ContainsKey(info.Name))
            //    data[info.Name] = (bool)attribute.StandardValue;
            //info.SetValue(this, (bool)data[info.Name]); 
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
