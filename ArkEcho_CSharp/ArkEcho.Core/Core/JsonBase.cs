using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        // TODO: Was wenn Class null oder Listen etc.

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
                    handlePrimitiveType<string>(Data, info, Mode);
                else if (info.PropertyType == typeof(bool))
                    handlePrimitiveType<bool>(Data, info, Mode);
                else if (info.PropertyType == typeof(Guid))
                    handlePrimitiveType<Guid>(Data, info, Mode);
                else if (info.PropertyType == typeof(DateTime))
                    handlePrimitiveType<DateTime>(Data, info, Mode);
                else if (info.PropertyType == typeof(TimeSpan))
                    handlePrimitiveType<TimeSpan>(Data, info, Mode);
                else if (info.PropertyType == typeof(uint))
                    handlePrimitiveType<uint>(Data, info, Mode);
                else if (info.PropertyType == typeof(int))
                    handlePrimitiveType<int>(Data, info, Mode);
                else if (info.PropertyType == typeof(double))
                    handlePrimitiveType<double>(Data, info, Mode);
                else if (info.PropertyType == typeof(long))
                    handlePrimitiveType<long>(Data, info, Mode);
                else if (info.PropertyType == typeof(float))
                    handlePrimitiveType<float>(Data, info, Mode);
                else if (info.PropertyType.IsClass)
                {
                    if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                        handleJsonClass(Data, info, Mode); // Recursion!
                    else if (IsICollection(info) && !info.PropertyType.IsArray)
                        handleCollection(Data, info, Mode);
                    else if (info.PropertyType.IsArray)
                        handleArray(Data, info, Mode);
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

        private void handleArray(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            Type arrayType = Info.PropertyType.GetElementType();

            if (arrayType == typeof(string))
                handlePrimitiveArray<string>(Data, Info, Mode);
            else if (arrayType == typeof(bool))
                handlePrimitiveArray<bool>(Data, Info, Mode);
            else if (arrayType == typeof(Guid))
                handlePrimitiveArray<Guid>(Data, Info, Mode);
            else if (arrayType == typeof(DateTime))
                handlePrimitiveArray<DateTime>(Data, Info, Mode);
            else if (arrayType == typeof(TimeSpan))
                handlePrimitiveArray<TimeSpan>(Data, Info, Mode);
            else if (arrayType == typeof(uint))
                handlePrimitiveArray<uint>(Data, Info, Mode);
            else if (arrayType == typeof(int))
                handlePrimitiveArray<int>(Data, Info, Mode);
            else if (arrayType == typeof(double))
                handlePrimitiveArray<double>(Data, Info, Mode);
            else if (arrayType == typeof(long))
                handlePrimitiveArray<long>(Data, Info, Mode);
            else if (arrayType == typeof(float))
                handlePrimitiveArray<float>(Data, Info, Mode);
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
                    else if (Mode == FillMode.PropertiesToJson)
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

        private void handleCollection(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            // TODO was wenn liste leer
            Type collectionType = Info.PropertyType.GenericTypeArguments[0];

            if (collectionType == typeof(string))
                handlePrimitiveCollection<string>(Data, Info, Mode);
            else if (collectionType == typeof(bool))
                handlePrimitiveCollection<bool>(Data, Info, Mode);
            else if (collectionType == typeof(Guid))
                handlePrimitiveCollection<Guid>(Data, Info, Mode);
            else if (collectionType == typeof(DateTime))
                handlePrimitiveCollection<DateTime>(Data, Info, Mode);
            else if (collectionType == typeof(TimeSpan))
                handlePrimitiveCollection<TimeSpan>(Data, Info, Mode);
            else if (collectionType == typeof(uint))
                handlePrimitiveCollection<uint>(Data, Info, Mode);
            else if (collectionType == typeof(int))
                handlePrimitiveCollection<int>(Data, Info, Mode);
            else if (collectionType == typeof(double))
                handlePrimitiveCollection<double>(Data, Info, Mode);
            else if (collectionType == typeof(long))
                handlePrimitiveCollection<long>(Data, Info, Mode);
            else if (collectionType == typeof(float))
                handlePrimitiveCollection<float>(Data, Info, Mode);
            else if (collectionType.IsClass)
            {
                if (collectionType.IsSubclassOf(typeof(JsonBase)))
                {
                    if (Mode == FillMode.JsonToProperties)
                    {
                        MethodInfo methAdd = Info.PropertyType.GetMethod("Add");

                        object icollection = Activator.CreateInstance(Info.PropertyType);

                        Info.SetValue(this, icollection);

                        JToken[] jArray = Data[Info.Name].ToArray();

                        for (int i = 0; i < jArray.Length; i++)
                        {
                            JsonBase instance = (JsonBase)Activator.CreateInstance(collectionType);
                            JObject obj = (JObject)jArray[i];
                            instance.handleProperties(obj, Mode);
                            methAdd.Invoke(icollection, new object[] { instance });
                        }
                    }
                    else if (Mode == FillMode.PropertiesToJson)
                    {
                        MethodInfo methToArray = Info.PropertyType.GetMethod("ToArray");

                        Array collectionArray = (Array)methToArray.Invoke(Info.GetValue(this), null);
                        JArray jArray = new JArray();

                        for (int i = 0; i < collectionArray.Length; i++)
                        {
                            JsonBase cls = (JsonBase)collectionArray.GetValue(i);
                            JObject obj = new JObject();
                            cls.handleProperties(obj, Mode);
                            jArray.Add(obj);
                        }

                        Data[Info.Name] = jArray;
                    }
                }
            }
        }

        private void handlePrimitiveCollection<T>(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Mode == FillMode.JsonToProperties)
            {
                MethodInfo methAdd = Info.PropertyType.GetMethod("Add");

                object icollection = Activator.CreateInstance(Info.PropertyType);

                Info.SetValue(this, icollection);

                JToken[] jArray = Data[Info.Name].ToArray();

                for (int i = 0; i < jArray.Length; i++)
                    methAdd.Invoke(icollection, new object[] { (T)jArray[i].ToObject(typeof(T)) });
            }
            else if (Mode == FillMode.PropertiesToJson)
            {
                MethodInfo methToArray = Info.PropertyType.GetMethod("ToArray");

                Array collectionArray = (Array)methToArray.Invoke(Info.GetValue(this), null);
                JArray jArray = new JArray();

                for (int i = 0; i < collectionArray.Length; i++)
                    jArray.Add((dynamic)(T)collectionArray.GetValue(i));

                Data[Info.Name] = jArray;
            }
        }

        private void handlePrimitiveArray<T>(JObject Data, PropertyInfo Info, FillMode Mode)
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

        private void handlePrimitiveType<T>(JObject Data, PropertyInfo Info, FillMode Mode)
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
