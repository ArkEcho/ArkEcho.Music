using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        private enum Mode
        {
            PropToJson = 0,
            JsonToProp
        }

        private enum Func
        {
            PrimType,
            PrimArray,
            PrimCollection
        }

        protected class JsonProperty : Attribute { }

        private string fileName = string.Empty;

        protected JsonBase()
        {
        }

        protected JsonBase(string FileName)
        {
            this.fileName = FileName;
        }

        public async Task<bool> LoadFromFile(string Folder, bool RewriteAddMissingParams = false)
        {
            string filepath = $"{Folder}\\{fileName}";

            Console.WriteLine($"Loading Config File {filepath}");

            string content = string.Empty;
            if (File.Exists(filepath))
                content = await File.ReadAllTextAsync(filepath);

            // Load Props from JSON
            bool foundCorrectExistingFile = await LoadFromJsonString(content);

            if (RewriteAddMissingParams)
                await SaveToFile(Folder);

            return foundCorrectExistingFile;
        }

        public async Task<bool> SaveToFile(string Folder)
        {
            string filepath = $"{Folder}\\{fileName}";

            try
            {
                string json = await SaveToJsonString();
                await File.WriteAllTextAsync(filepath, json, System.Text.Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on saving JSON to File: {ex.Message}");
                return false;
            }
            return true;
        }

        public async Task<string> SaveToJsonString()
        {
            JObject data = new JObject();
            await Task.Factory.StartNew(() => handleProperties(data, Mode.PropToJson));
            return data.ToString();
        }

        public async Task<bool> LoadFromJsonString(string Json)
        {
            // TODO: What if already set?
            JObject data = null;

            if (!string.IsNullOrEmpty(Json))
            {
                try
                {
                    data = await Task.Factory.StartNew(() => data = JObject.Parse(Json));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception on parsing the JSON: {ex.Message}");
                }
            }

            if (data == null)
                return false;

            await Task.Factory.StartNew(() => handleProperties(data, Mode.JsonToProp));
            return true;
        }

        private void handleProperties(JObject Data, Mode Mode)
        {
            foreach (PropertyInfo info in getJsonProperties(Data, Mode))
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

        private void handleJsonClass(JObject Data, PropertyInfo Info, Mode Mode)
        {
            if (Mode == Mode.JsonToProp)
            {
                JsonBase instance = (JsonBase)Activator.CreateInstance(Info.PropertyType);
                Info.SetValue(this, instance);
                instance.handleProperties((JObject)Data[Info.Name], Mode); // Recursion
            }
            else if (Mode == Mode.PropToJson)
                Data[Info.Name] = makeJObjFromJBaseClass(Info.GetValue(this), Mode);
        }

        private void handlePrimitiveType<T>(JObject Data, PropertyInfo Info, Mode Mode)
        {
            if (Mode == Mode.JsonToProp)
                Info.SetValue(this, Convert.ChangeType(Data[Info.Name], typeof(T)));
            else if (Mode == Mode.PropToJson)
                Data[Info.Name] = (dynamic)(T)Info.GetValue(this);
        }

        private List<PropertyInfo> getJsonProperties(JObject Data, Mode Mode)
        {
            List<PropertyInfo> result = this.GetType().GetProperties().ToList().FindAll(x => x.GetCustomAttributes().ToList().Find(y => y is JsonProperty) != null);
            if (Mode == Mode.JsonToProp) result.RemoveAll(x => !Data.ContainsKey(x.Name));
            else if (Mode == Mode.PropToJson) result.RemoveAll(x => x.GetValue(this) == null);
            return result;
        }

        private bool isAllowedCollection(PropertyInfo Info)
        {
            // TODO bessere Lösung -> SortedSet on Playlist?
            return Info.PropertyType.UnderlyingSystemType.Name.Equals(typeof(List<>).Name, StringComparison.OrdinalIgnoreCase)
                && Info.PropertyType.GenericTypeArguments.Length == 1;
        }

        private void handleArray(JObject Data, PropertyInfo Info, Mode Mode)
        {
            Type arrayType = Info.PropertyType.GetElementType();

            if (checkPrimitiveTypeAndFunction(arrayType, Func.PrimArray, Data, Info, Mode))
            { /* Done */ }
            else if (arrayType.IsClass && arrayType.IsSubclassOf(typeof(JsonBase)))
            {
                if (Mode == Mode.JsonToProp)
                {
                    prepareJArrayToArray(Data, Info, arrayType, out JToken[] jArray, out Array propArray);

                    for (int i = 0; i < jArray.Length; i++)
                    {
                        JsonBase instance = (JsonBase)Activator.CreateInstance(arrayType);
                        JObject obj = (JObject)jArray[i];
                        instance.handleProperties(obj, Mode);
                        propArray.SetValue(instance, i);
                    }
                }
                else if (Mode == Mode.PropToJson)
                {
                    Array propArray = (Array)Info.GetValue(this);
                    JArray jArray = new JArray();

                    for (int i = 0; i < propArray.Length; i++)
                        jArray.Add(makeJObjFromJBaseClass(propArray.GetValue(i), Mode));

                    Data[Info.Name] = jArray;
                }
            }
        }

        private void handlePrimitiveArray<T>(JObject Data, PropertyInfo Info, Mode Mode)
        {
            if (Mode == Mode.JsonToProp)
            {
                prepareJArrayToArray(Data, Info, typeof(T), out JToken[] jArray, out Array propArray);

                for (int i = 0; i < jArray.Length; i++)
                    propArray.SetValue((T)jArray[i].ToObject(typeof(T)), i);
            }
            else if (Mode == Mode.PropToJson)
            {
                Array propArray = (Array)Info.GetValue(this);
                JArray jArray = new JArray();

                for (int i = 0; i < propArray.Length; i++)
                    jArray.Add((dynamic)(T)propArray.GetValue(i));

                Data[Info.Name] = jArray;
            }
        }

        private void prepareJArrayToArray(JObject Data, PropertyInfo Info, Type Typ, out JToken[] JArray, out Array PropArray)
        {
            JArray = Data[Info.Name].ToArray();
            PropArray = Array.CreateInstance(Typ, JArray.Length);
            Info.SetValue(this, PropArray);
        }

        private void handleCollection(JObject Data, PropertyInfo Info, Mode Mode)
        {
            Type collectionType = Info.PropertyType.GenericTypeArguments[0];

            if (checkPrimitiveTypeAndFunction(collectionType, Func.PrimCollection, Data, Info, Mode))
            { /* Done */ }
            else if (collectionType.IsClass && collectionType.IsSubclassOf(typeof(JsonBase)))
            {
                if (Mode == Mode.JsonToProp)
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
                else if (Mode == Mode.PropToJson)
                {
                    prepareCollectionToJArray(Info, out Array collectionArray, out JArray jArray);

                    for (int i = 0; i < collectionArray.Length; i++)
                        jArray.Add(makeJObjFromJBaseClass(collectionArray.GetValue(i), Mode));

                    Data[Info.Name] = jArray;
                }
            }
        }

        private void handlePrimitiveCollection<T>(JObject Data, PropertyInfo Info, Mode Mode)
        {
            if (Mode == Mode.JsonToProp)
            {
                prepareJArrayToCollection(Info, Data, out MethodInfo methAdd, out object icollection, out JToken[] jArray);

                for (int i = 0; i < jArray.Length; i++)
                    methAdd.Invoke(icollection, new object[] { (T)jArray[i].ToObject(typeof(T)) });
            }
            else if (Mode == Mode.PropToJson)
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

        private JObject makeJObjFromJBaseClass(object Object, Mode Mode)
        {
            JsonBase cls = (JsonBase)Object;
            JObject jObj = new JObject();
            cls.handleProperties(jObj, Mode); // Recursion
            return jObj;
        }

        // TODO: Uri?
        private bool checkPrimitiveTypeAndFunction(Type Type, Func Function, JObject Data, PropertyInfo Info, Mode Mode)
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
