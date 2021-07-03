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

        private enum Func
        {
            PrimType,
            PrimArray,
            PrimCollection
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
                catch (Exception) { }
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

                if (checkPrimitiveTypeAndFunction(info.PropertyType, Func.PrimType, Data, info, Mode))
                { /* Done */ }
                else if (info.PropertyType.IsClass)
                {
                    if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                        handleJsonClass(Data, info, Mode); // Recursion!
                    else if (isAllowedCollection(info))
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

        private bool isAllowedCollection(PropertyInfo Info)
        {
            // TODO bessere Lösung?
            return Info.PropertyType.UnderlyingSystemType.Name.Equals(typeof(List<>).Name, StringComparison.OrdinalIgnoreCase);
        }

        private void handleArray(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            Type arrayType = Info.PropertyType.GetElementType();

            if (checkPrimitiveTypeAndFunction(arrayType, Func.PrimArray, Data, Info, Mode))
            { /* Done */ }
            else if (arrayType.IsClass && arrayType.IsSubclassOf(typeof(JsonBase)))
            {
                if (Mode == FillMode.JsonToProperties)
                {
                    prepareJArrayToArray(Data, Info, arrayType, out JToken[] jArray, out Array PropArray);

                    for (int i = 0; i < PropArray.Length; i++)
                    {
                        JsonBase instance = (JsonBase)Activator.CreateInstance(arrayType);
                        JObject obj = (JObject)jArray[i];
                        instance.handleProperties(obj, Mode);
                        PropArray.SetValue(instance, i);
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
                Type typ = typeof(T);
                prepareJArrayToArray(Data, Info, typ, out JToken[] jArray, out Array PropArray);

                for (int i = 0; i < PropArray.Length; i++)
                    PropArray.SetValue((T)jArray[i].ToObject(typ), i);
            }
            else if (Mode == FillMode.PropertiesToJson)
            {
                Array arrayProp = (Array)Info.GetValue(this);
                JArray jArray = new JArray();

                for (int i = 0; i < arrayProp.Length; i++)
                    jArray.Add((dynamic)(T)arrayProp.GetValue(i));

                Data[Info.Name] = jArray;
            }
        }

        private void prepareJArrayToArray(JObject Data, PropertyInfo Info, Type Typ, out JToken[] jArray, out Array PropArray)
        {
            jArray = Data[Info.Name].ToArray();
            PropArray = Array.CreateInstance(Typ, jArray.Length);
            Info.SetValue(this, PropArray);
        }

        private void handleCollection(JObject Data, PropertyInfo Info, FillMode Mode)
        {
            // TODO was wenn liste leer
            Type collectionType = Info.PropertyType.GenericTypeArguments[0];

            if (checkPrimitiveTypeAndFunction(collectionType, Func.PrimCollection, Data, Info, Mode))
            { /* Done */ }
            else if (collectionType.IsClass && collectionType.IsSubclassOf(typeof(JsonBase)))
            {
                if (Mode == FillMode.JsonToProperties)
                {
                    prepareJArrayToCollection(Info, Data, out MethodInfo methAdd, out object icollection, out JToken[] jArray);

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
                    prepareCollectionToJArray(Info, out Array collectionArray, out JArray jArray);

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
                prepareJArrayToCollection(Info, Data, out MethodInfo methAdd, out object icollection, out JToken[] jArray);

                for (int i = 0; i < jArray.Length; i++)
                    methAdd.Invoke(icollection, new object[] { (T)jArray[i].ToObject(typeof(T)) });
            }
            else if (Mode == FillMode.PropertiesToJson)
            {
                prepareCollectionToJArray(Info, out Array collectionArray, out JArray jArray);

                for (int i = 0; i < collectionArray.Length; i++)
                    jArray.Add((dynamic)(T)collectionArray.GetValue(i));

                Data[Info.Name] = jArray;
            }
        }

        private void prepareJArrayToCollection(PropertyInfo Info, JObject Data, out MethodInfo MethodAdd, out object Collection, out JToken[] JArray)
        {
            MethodAdd = Info.PropertyType.GetMethod("Add");
            Collection = Activator.CreateInstance(Info.PropertyType);
            Info.SetValue(this, Collection);
            JArray = Data[Info.Name].ToArray();
        }

        private void prepareCollectionToJArray(PropertyInfo Info, out Array CollectionArray, out JArray jArray)
        {
            MethodInfo methToArray = Info.PropertyType.GetMethod("ToArray");
            CollectionArray = (Array)methToArray.Invoke(Info.GetValue(this), null);
            jArray = new JArray();
        }

        private JObject makeJObjFromJBaseClass(object Object, FillMode Mode)
        {
            JsonBase cls = (JsonBase)Object;
            JObject jObj = new JObject();
            cls.handleProperties(jObj, Mode); // Recursion
            return jObj;
        }

        private bool checkPrimitiveTypeAndFunction(Type Type, Func Function, JObject Data, PropertyInfo Info, FillMode Mode)
        {
            if (Type == typeof(string))
            {
                if (Function == Func.PrimType) handlePrimitiveType<string>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<string>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<string>(Data, Info, Mode);
            }
            else if (Type == typeof(bool))
            {
                if (Function == Func.PrimType) handlePrimitiveType<bool>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<bool>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<bool>(Data, Info, Mode);
            }
            else if (Type == typeof(Guid))
            {
                if (Function == Func.PrimType) handlePrimitiveType<Guid>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<Guid>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<Guid>(Data, Info, Mode);
            }
            else if (Type == typeof(DateTime))
            {
                if (Function == Func.PrimType) handlePrimitiveType<DateTime>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<DateTime>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<DateTime>(Data, Info, Mode);
            }
            else if (Type == typeof(TimeSpan))
            {
                if (Function == Func.PrimType) handlePrimitiveType<TimeSpan>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<TimeSpan>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<TimeSpan>(Data, Info, Mode);
            }
            else if (Type == typeof(uint))
            {
                if (Function == Func.PrimType) handlePrimitiveType<uint>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<uint>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<uint>(Data, Info, Mode);
            }
            else if (Type == typeof(int))
            {
                if (Function == Func.PrimType) handlePrimitiveType<int>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<int>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<int>(Data, Info, Mode);
            }
            else if (Type == typeof(double))
            {
                if (Function == Func.PrimType) handlePrimitiveType<double>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<double>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<double>(Data, Info, Mode);
            }
            else if (Type == typeof(long))
            {
                if (Function == Func.PrimType) handlePrimitiveType<long>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<long>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<long>(Data, Info, Mode);
            }
            else if (Type == typeof(float))
            {
                if (Function == Func.PrimType) handlePrimitiveType<float>(Data, Info, Mode);
                else if (Function == Func.PrimArray) handlePrimitiveArray<float>(Data, Info, Mode);
                else if (Function == Func.PrimCollection) handlePrimitiveCollection<float>(Data, Info, Mode);
            }
            else
                return false; // Not a supported Primitive Type

            return true;
        }
    }
}
