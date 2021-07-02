using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        // TODO: Was wenn Class oder JsonFeld oder Propnull

        private enum FillMode
        {
            PropertiesToJson = 0,
            JsonToProperties
        }

        private enum HandlePrimitiveFunction
        {
            HandlePrimitiveType,
            HandlePrimitiveArray,
            HandlePrimitiveCollection
        }

        protected class JsonProperty : Attribute { }

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

                if (PrimitiveTypeFunction(info.PropertyType, HandlePrimitiveFunction.HandlePrimitiveType, Data, info, Mode))
                {
                    // WHAT
                }
                else if (info.PropertyType.IsClass)
                {
                    if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                        handleJsonClass(Data, info, Mode); // Recursion!
                    else if (IsAllowedCollection(info))
                        handleCollection(Data, info, Mode);
                    else if (info.PropertyType.IsArray)
                        handleArray(Data, info, Mode);
                }
            }
        }

        private void handleJsonClass(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Mode == FillMode.JsonToProperties)
            {
                JsonBase instance = (JsonBase)Activator.CreateInstance(Info.PropertyType);
                Info.SetValue(this, instance);

                if (Data.ContainsKey(Info.Name))
                    instance.handleProperties((JObject)Data[Info.Name], Mode); // Recursion
            }
            else if (Mode == FillMode.PropertiesToJson)
                Data[Info.Name] = makeJObjFromJBaseClass(Info.GetValue(this), Mode);
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
        }

        private List<PropertyInfo> getJsonProperties()
        {
            return this.GetType().GetProperties().ToList().FindAll(x => x.GetCustomAttributes().ToList().Find(y => y is JsonProperty) != null);
        }

        private bool IsAllowedCollection(PropertyInfo Info)
        {
            // TODO bessere Lösung?
            return Info.PropertyType.UnderlyingSystemType.Name.Equals(typeof(List<>).Name, StringComparison.OrdinalIgnoreCase);
        }

        private void handleArray(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            Type arrayType = Info.PropertyType.GetElementType();

            if (PrimitiveTypeFunction(arrayType, HandlePrimitiveFunction.HandlePrimitiveArray, Data, Info, Mode))
            {
                // WHAT
            }
            else if (arrayType.IsClass && arrayType.IsSubclassOf(typeof(JsonBase)))
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
                        jArray.Add(makeJObjFromJBaseClass(arr.GetValue(i), Mode));

                    Data[Info.Name] = jArray;
                }
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

        private void handleCollection(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            // TODO was wenn liste leer
            Type collectionType = Info.PropertyType.GenericTypeArguments[0];

            if (PrimitiveTypeFunction(collectionType, HandlePrimitiveFunction.HandlePrimitiveCollection, Data, Info, Mode))
            {
                // WHAT
            }
            else if (collectionType.IsClass && collectionType.IsSubclassOf(typeof(JsonBase)))
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
                        jArray.Add(makeJObjFromJBaseClass(collectionArray.GetValue(i), Mode));

                    Data[Info.Name] = jArray;
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

        private JObject makeJObjFromJBaseClass(object Object, FillMode Mode)
        {
            JsonBase cls = (JsonBase)Object;
            JObject jObj = new JObject();
            cls.handleProperties(jObj, Mode); // Recursion
            return jObj;
        }

        private bool PrimitiveTypeFunction(Type Type, HandlePrimitiveFunction Function, JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Type == typeof(string))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<string>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<string>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<string>(Data, Info, Mode);
            }
            else if (Type == typeof(bool))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<bool>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<bool>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<bool>(Data, Info, Mode);
            }
            else if (Type == typeof(Guid))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<Guid>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<Guid>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<Guid>(Data, Info, Mode);
            }
            else if (Type == typeof(DateTime))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<DateTime>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<DateTime>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<DateTime>(Data, Info, Mode);
            }
            else if (Type == typeof(TimeSpan))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<TimeSpan>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<TimeSpan>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<TimeSpan>(Data, Info, Mode);
            }
            else if (Type == typeof(uint))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<uint>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<uint>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<uint>(Data, Info, Mode);
            }
            else if (Type == typeof(int))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<int>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<int>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<int>(Data, Info, Mode);
            }
            else if (Type == typeof(double))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<double>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<double>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<double>(Data, Info, Mode);
            }
            else if (Type == typeof(long))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<long>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<long>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<long>(Data, Info, Mode);
            }
            else if (Type == typeof(float))
            {
                if (Function == HandlePrimitiveFunction.HandlePrimitiveType)
                    handlePrimitiveType<float>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveArray)
                    handlePrimitiveArray<float>(Data, Info, Mode);
                else if (Function == HandlePrimitiveFunction.HandlePrimitiveCollection)
                    handlePrimitiveCollection<float>(Data, Info, Mode);
            }
            else
                return false; // Not a supported Primitive Type

            return true;
        }
    }
}
