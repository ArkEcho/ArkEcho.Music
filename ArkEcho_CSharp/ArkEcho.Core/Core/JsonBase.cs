using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        // TODO: Listen
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
                    else if (IsICollection(info) || info.PropertyType.IsArray)
                        handleCollectionArray(Data, info, Mode);
                }
            }
        }

        private List<PropertyInfo> getJsonProperties()
        {
            return this.GetType().GetProperties().ToList().FindAll(x => x.GetCustomAttributes().ToList().Find(y => y is JsonProperty) != null);
        }

        private bool IsICollection(PropertyInfo Info)
        {
            return Info.PropertyType.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        private void handleCollectionArray(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Info.PropertyType.IsArray)
            {
                Type arrayType = Info.PropertyType.GetElementType();

                if (arrayType == typeof(string))
                    handleGenericArray<string>(Data, Info, Mode);
                else if (arrayType == typeof(bool))
                    handleGenericArray<bool>(Data, Info, Mode);
                else if (arrayType == typeof(Guid))
                    handleGenericArray<Guid>(Data, Info, Mode);
                else if (arrayType == typeof(DateTime))
                    handleGenericArray<DateTime>(Data, Info, Mode);
                else if (arrayType == typeof(TimeSpan))
                    handleGenericArray<TimeSpan>(Data, Info, Mode);
                else if (arrayType == typeof(uint))
                    handleGenericArray<uint>(Data, Info, Mode);
                else if (arrayType == typeof(int))
                    handleGenericArray<int>(Data, Info, Mode);
                else if (arrayType == typeof(double))
                    handleGenericArray<double>(Data, Info, Mode);
                else if (arrayType == typeof(long))
                    handleGenericArray<long>(Data, Info, Mode);
                else if (arrayType == typeof(float))
                    handleGenericArray<float>(Data, Info, Mode);
                else if (arrayType.IsClass)
                {
                    if (arrayType.IsSubclassOf(typeof(JsonBase)))
                    {
                        if (Mode == FillMode.JsonToProperties)
                        {
                            JToken[] jArray = Data[Info.Name].ToArray();
                            Array arrayProp = Array.CreateInstance(arrayType, jArray.Length);

                            Info.SetValue(this, arrayProp);

                            for (int i = 0; i < arrayProp.Length; i++)
                            {
                                JsonBase instance = (JsonBase)Activator.CreateInstance(arrayType);
                                JObject obj = (JObject)jArray[i];
                                instance.handleProperties(obj, Mode);
                                arrayProp.SetValue(instance, i);
                            }
                        }
                        else
                        {
                            Array arr = (Array)Info.GetValue(this);
                            JArray jArray = new JArray();

                            for (int i = 0; i < arr.Length; i++)
                            {
                                JsonBase cls = (JsonBase)arr.GetValue(i);
                                JObject obj = new JObject();
                                cls.handleProperties(obj, Mode);
                                jArray.Add(obj);
                            }

                            Data[Info.Name] = jArray;
                        }
                    }
                }
            }
            else
            {
                // TODO
            }
        }

        private void handleGenericArray<T>(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Mode == FillMode.JsonToProperties)
            {
                JToken[] jArray = Data[Info.Name].ToArray();

                T[] arrayProp = new T[jArray.Length];
                Info.SetValue(this, arrayProp);
                for (int i = 0; i < arrayProp.Length; i++)
                    arrayProp[i] = (T)jArray[i].ToObject(typeof(T));
            }
            else if (Mode == FillMode.PropertiesToJson)
            {
                T[] arrayProp = (T[])Info.GetValue(this);
                JArray jArray = new JArray();
                for (int i = 0; i < arrayProp.Length; i++)
                    jArray.Add((dynamic)(T)arrayProp[i]);
                Data[Info.Name] = jArray;
            }
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
                    JObject obj = new JObject();
                    cls.handleProperties(obj, Mode);
                    Data[Info.Name] = obj;
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
