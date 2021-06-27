using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        private enum FillMode
        {
            PropertiesToJson = 0,
            JsonToProperties
        }

        protected class JsonProperty : Attribute
        {
            //public object StandardValue { get; set; } = string.Empty;
        }

        protected string GetJsonAsString()
        {
            JObject data = new JObject();

            setProperties(data, FillMode.PropertiesToJson);

            return data.ToString().Replace("\\\\", "\\");
        }

        protected bool LoadPropertiesFromJsonString(string Json)
        {
            JObject data = null;

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
            {
                setProperties(data, FillMode.JsonToProperties);
                return true;
            }
            else
                return false;
        }

        private void setProperties(JObject Data, FillMode Mode)
        {
            foreach (PropertyInfo info in getJsonProperties())
            {
                if (info.PropertyType == typeof(string))
                    setProperty<string>(Data, info, Mode);
                else if (info.PropertyType == typeof(bool))
                    setProperty<bool>(Data, info, Mode);
                else if (info.PropertyType == typeof(Guid))
                    setProperty<Guid>(Data, info, Mode);
                else if (info.PropertyType == typeof(DateTime))
                    setProperty<DateTime>(Data, info, Mode);
                else if (info.PropertyType == typeof(TimeSpan))
                    setProperty<TimeSpan>(Data, info, Mode);
                else if (info.PropertyType == typeof(uint))
                    setProperty<uint>(Data, info, Mode);
                else if (info.PropertyType == typeof(int))
                    setProperty<int>(Data, info, Mode);
                else if (info.PropertyType == typeof(double))
                    setProperty<double>(Data, info, Mode);
                else if (info.PropertyType == typeof(long))
                    setProperty<long>(Data, info, Mode);
                else if (info.PropertyType == typeof(float))
                    setProperty<float>(Data, info, Mode);
                else if (info.PropertyType.IsClass)
                {
                    if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                    {
                        JsonBase instance = (JsonBase)Activator.CreateInstance(info.PropertyType);
                        info.SetValue(this, instance);

                        if (Data.ContainsKey(info.Name))
                            instance.setProperties((JObject)Data[info.Name], Mode);
                    }
                    //else if (info.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>)))
                    //{
                    //    var instance = Activator.CreateInstance(info.PropertyType);
                    //    info.SetValue(this, instance);

                    //    if (!data.ContainsKey(info.Name))
                    //        data[info.Name] = new JObject();

                    //    //data[info.Name].AsEnumerable().ToList().ForEach(x => x.ToObject());

                    //    info.PropertyType.GetMethod("Add").Invoke(instance, new object[] { Guid.NewGuid() });
                    //}
                }
                //else if (info.PropertyType.IsArray)
                //{
                //    // TODO
                //    int[] bla = new int[10];

                //}
            }
        }

        private List<PropertyInfo> getJsonProperties()
        {
            return this.GetType().GetProperties().ToList().FindAll(x => x.GetCustomAttributes().ToList().Find(y => y is JsonProperty) != null);
        }

        private void setProperty<T>(JObject Data, PropertyInfo info, FillMode Mode)
        {
            if (Data.ContainsKey(info.Name)) // If key doesnt exist, set Standardvalue
                                             //Data[info.Name] = (dynamic)(T)attribute.StandardValue;
                info.SetValue(this, Convert.ChangeType(Data[info.Name], typeof(T)));

            //if (!data.ContainsKey(info.Name))
            //    data[info.Name] = (bool)attribute.StandardValue;
            //info.SetValue(this, (bool)data[info.Name]); 
        }

    }
}
