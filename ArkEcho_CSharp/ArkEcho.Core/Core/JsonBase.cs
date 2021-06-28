using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        // TODO: Listen und Arrays
        // TODO: Was wenn Class null
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

            handleProperties(data, FillMode.PropertiesToJson);

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
                handleProperties(data, FillMode.JsonToProperties);
                return true;
            }
            else
                return false;
        }

        private void handleProperties(JObject Data, FillMode Mode)
        {
            foreach (PropertyInfo info in getJsonProperties())
            {
                if (info.PropertyType == typeof(string))
                    handleGenericType<string>(Data, info, Mode);
                else if (info.PropertyType == typeof(bool))
                    handleGenericType<bool>(Data, info, Mode);
                else if (info.PropertyType == typeof(Guid))
                    handleGenericType<Guid>(Data, info, Mode);
                else if (info.PropertyType == typeof(DateTime))
                    handleGenericType<DateTime>(Data, info, Mode);
                else if (info.PropertyType == typeof(TimeSpan))
                    handleGenericType<TimeSpan>(Data, info, Mode);
                else if (info.PropertyType == typeof(uint))
                    handleGenericType<uint>(Data, info, Mode);
                else if (info.PropertyType == typeof(int))
                    handleGenericType<int>(Data, info, Mode);
                else if (info.PropertyType == typeof(double))
                    handleGenericType<double>(Data, info, Mode);
                else if (info.PropertyType == typeof(long))
                    handleGenericType<long>(Data, info, Mode);
                else if (info.PropertyType == typeof(float))
                    handleGenericType<float>(Data, info, Mode);
                else if (info.PropertyType.IsClass)
                {
                    if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                        handleJsonClass(Data, info, Mode); // Recursion!

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

        private void handleJsonClass(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Mode == FillMode.JsonToProperties)
            {
                JsonBase instance = (JsonBase)Activator.CreateInstance(Info.PropertyType);
                Info.SetValue(this, instance);

                if (Data.ContainsKey(Info.Name))
                    instance.handleProperties((JObject)Data[Info.Name], Mode); // Recursion!
            }
            else if (Mode == FillMode.PropertiesToJson)
            {
                JsonBase cls = (JsonBase)Info.GetValue(this);
                if (cls != null)
                {
                    JObject jCls = new JObject();
                    cls.handleProperties(jCls, Mode);
                    Data[Info.Name] = jCls;
                }
            }
        }

        private void handleGenericType<T>(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Mode == FillMode.JsonToProperties)
            {
                if (Data.ContainsKey(Info.Name))
                    Info.SetValue(this, Convert.ChangeType(Data[Info.Name], typeof(T)));
            }
            else if (Mode == FillMode.PropertiesToJson)
                Data[Info.Name] = (dynamic)(T)Info.GetValue(this);

            //if (!data.ContainsKey(info.Name))
            //    data[info.Name] = (bool)attribute.StandardValue;
            //info.SetValue(this, (bool)data[info.Name]); 
        }

    }
}
