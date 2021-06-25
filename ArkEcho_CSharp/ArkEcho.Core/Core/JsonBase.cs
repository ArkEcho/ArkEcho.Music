using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        protected class JsonProperty : Attribute
        {
            public object StandardValue { get; set; } = string.Empty;
        }

        private JObject data { get; set; } = null;

        protected string GetJsonAsString()
        {
            return data.ToString().Replace("\\\\", "\\");
        }

        protected bool LoadJsonFromStringAndSetProperties(string Json)
        {
            bool correctJson = false;

            if (!string.IsNullOrEmpty(Json))
            {
                Json = Json.Replace("\\", "\\\\");
                try
                {
                    data = JObject.Parse(Json);
                }
                catch (Exception)
                {
                    Console.WriteLine($"### Exception parsing Config File!");
                }
            }

            if (data != null)
                correctJson = true;
            else
                data = new JObject();

            loadProperties();

            return correctJson;
        }

        private void loadProperties()
        {
            foreach (PropertyInfo info in this.GetType().GetProperties())
            {
                foreach (object attr in info.GetCustomAttributes(true))
                {
                    JsonProperty attribute = (JsonProperty)attr;
                    if (attribute != null)
                    {
                        if (info.PropertyType == typeof(string))
                            checkKeyAndSetProperty<string>(attribute, info);
                        else if (info.PropertyType == typeof(bool))
                            checkKeyAndSetProperty<bool>(attribute, info);
                        else if (info.PropertyType == typeof(Guid))
                            checkKeyAndSetProperty<Guid>(attribute, info);
                        else if (info.PropertyType == typeof(int))
                            checkKeyAndSetProperty<int>(attribute, info);
                        else if (info.PropertyType == typeof(double))
                            checkKeyAndSetProperty<double>(attribute, info);
                        // TODO
                        //else if (info.PropertyType.IsClass && info.PropertyType.IsSubclassOf())
                        //{
                        //  foreach (JProperty prop in data.Properties())
                        //  {
                        //      if (prop.Value.Type == JTokenType.Object)
                        //      {
                        //          JObject ib = (JObject)prop.Value;
                        //          int testint = (int)ib["TestInt"];
                        //          double testdouble = (double)ib["TestDouble"];
                        //      }
                        //  }
                        //}
                    }
                }
            }
        }

        private void checkKeyAndSetProperty<T>(JsonProperty attribute, PropertyInfo info)
        {
            if (!data.ContainsKey(info.Name)) // If key doesnt exist, set Standardvalue
                data[info.Name] = (dynamic)(T)attribute.StandardValue;
            info.SetValue(this, Convert.ChangeType(data[info.Name], typeof(T)));

            //if (!data.ContainsKey(info.Name))
            //    data[info.Name] = (bool)attribute.StandardValue;
            //info.SetValue(this, (bool)data[info.Name]); 
        }

    }
}
