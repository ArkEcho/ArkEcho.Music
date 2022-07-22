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
        private enum JsonHandlingMode
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
            bool foundCorrectExistingFile = false;

            Console.WriteLine($"Loading Config File {filepath}");

            if (File.Exists(filepath))
            {
                string content = await File.ReadAllTextAsync(filepath);

                // Load Props from JSON
                foundCorrectExistingFile = await LoadFromJsonString(content);
            }

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
            JObject jsonData = new JObject();
            await Task.Factory.StartNew(() => handleProperties(jsonData, JsonHandlingMode.PropertiesToJson));

            string jsonString = jsonData.ToString().UnEscapeString();
            return jsonString;
        }

        public async Task<bool> LoadFromJsonString(string jsonString)
        {
            JObject jsonData = null;

            if (string.IsNullOrEmpty(jsonString))
                return false;

            try
            {
                jsonString = jsonString.EscapeString(); // Escape URI Backslashes etc.
                jsonData = await Task.Factory.StartNew(() => jsonData = JObject.Parse(jsonString));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception on parsing the JSON: {ex.Message}");
            }

            if (jsonData == null)
                return false;

            await Task.Factory.StartNew(() => handleProperties(jsonData, JsonHandlingMode.JsonToProperties));
            return true;
        }

        /// <summary>
        /// Entry Point for the JSON Handling. Sets the Properties to the given JOBject/JSON 
        /// or loads the Properties into the class - dependent on the given Mode.
        /// Called recursively, if the given JSON contains Class Objects.
        /// </summary>
        /// <param name="jsonData"></param>
        /// <param name="handlingMode"></param>
        private void handleProperties(JObject jsonData, JsonHandlingMode handlingMode)
        {
            foreach (PropertyInfo info in getJsonProperties(jsonData, handlingMode))
            {
                if (checkPrimitiveTypeAndFunction(jsonData, info, info.PropertyType, Func.PrimType, handlingMode))
                { /* Done */ }
                else if (info.PropertyType.IsClass)
                {
                    if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                        handleJsonClass(jsonData, info, handlingMode);
                    else if (isAllowedCollection(info))
                        handleCollection(jsonData, info, handlingMode);
                    else if (info.PropertyType.IsArray)
                        handleArray(jsonData, info, handlingMode);
                }
                else if (info.PropertyType.IsEnum)
                    handleEnum(jsonData, info, handlingMode);
            }
        }

        private void handleEnum(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            if (handlingMode == JsonHandlingMode.JsonToProperties)
            {
                if (Enum.TryParse(propInfo.PropertyType, jsonData[propInfo.Name].ToString(), out object result))
                    propInfo.SetValue(this, result);
            }
            else if (handlingMode == JsonHandlingMode.PropertiesToJson)
                jsonData[propInfo.Name] = (dynamic)(int)propInfo.GetValue(this); // Enums as int
        }

        private void handleJsonClass(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            if (handlingMode == JsonHandlingMode.JsonToProperties)
            {
                JsonBase instance = (JsonBase)Activator.CreateInstance(propInfo.PropertyType);
                propInfo.SetValue(this, instance);
                instance.handleProperties((JObject)jsonData[propInfo.Name], handlingMode);
            }
            else if (handlingMode == JsonHandlingMode.PropertiesToJson)
                jsonData[propInfo.Name] = makeJObjFromJBaseClass(propInfo.GetValue(this), handlingMode);
        }

        private void handlePrimitiveType<T>(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            if (handlingMode == JsonHandlingMode.JsonToProperties)
                propInfo.SetValue(this, Convert.ChangeType(jsonData[propInfo.Name], typeof(T)));
            else if (handlingMode == JsonHandlingMode.PropertiesToJson)
                jsonData[propInfo.Name] = (dynamic)(T)propInfo.GetValue(this);
        }

        private List<PropertyInfo> getJsonProperties(JObject jsonData, JsonHandlingMode handlingMode)
        {
            List<PropertyInfo> result = this.GetType().GetProperties().ToList().FindAll(x => x.GetCustomAttributes().ToList().Find(y => y is JsonProperty) != null);
            if (handlingMode == JsonHandlingMode.JsonToProperties) result.RemoveAll(x => !jsonData.ContainsKey(x.Name));
            else if (handlingMode == JsonHandlingMode.PropertiesToJson) result.RemoveAll(x => x.GetValue(this) == null);
            return result;
        }

        private bool isAllowedCollection(PropertyInfo propInfo)
        {
            return propInfo.PropertyType.UnderlyingSystemType.FullName.Contains("System.Collections.Generic",
                StringComparison.OrdinalIgnoreCase)
                && propInfo.PropertyType.GenericTypeArguments.Length == 1;
        }

        private void handleArray(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            Type arrayType = propInfo.PropertyType.GetElementType();

            if (checkPrimitiveTypeAndFunction(jsonData, propInfo, arrayType, Func.PrimArray, handlingMode))
            { /* Done */ }
            else if (arrayType.IsClass && arrayType.IsSubclassOf(typeof(JsonBase)))
            {
                if (handlingMode == JsonHandlingMode.JsonToProperties)
                {
                    prepareJArrayToArray(jsonData, propInfo, arrayType, out JToken[] jArray, out Array propArray);

                    for (int i = 0; i < jArray.Length; i++)
                    {
                        JsonBase instance = (JsonBase)Activator.CreateInstance(arrayType);
                        JObject obj = (JObject)jArray[i];
                        instance.handleProperties(obj, handlingMode);
                        propArray.SetValue(instance, i);
                    }
                }
                else if (handlingMode == JsonHandlingMode.PropertiesToJson)
                {
                    Array propArray = (Array)propInfo.GetValue(this);
                    JArray jArray = new JArray();

                    for (int i = 0; i < propArray.Length; i++)
                        jArray.Add(makeJObjFromJBaseClass(propArray.GetValue(i), handlingMode));

                    jsonData[propInfo.Name] = jArray;
                }
            }
        }

        private void handlePrimitiveArray<T>(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            if (handlingMode == JsonHandlingMode.JsonToProperties)
            {
                prepareJArrayToArray(jsonData, propInfo, typeof(T), out JToken[] jArray, out Array propArray);

                for (int i = 0; i < jArray.Length; i++)
                    propArray.SetValue((T)jArray[i].ToObject(typeof(T)), i);
            }
            else if (handlingMode == JsonHandlingMode.PropertiesToJson)
            {
                Array propArray = (Array)propInfo.GetValue(this);
                JArray jArray = new JArray();

                for (int i = 0; i < propArray.Length; i++)
                    jArray.Add((dynamic)(T)propArray.GetValue(i));

                jsonData[propInfo.Name] = jArray;
            }
        }

        private void prepareJArrayToArray(JObject jsonData, PropertyInfo propInfo, Type arrayType, out JToken[] jArray, out Array propArray)
        {
            jArray = jsonData[propInfo.Name].ToArray();
            propArray = Array.CreateInstance(arrayType, jArray.Length);
            propInfo.SetValue(this, propArray);
        }

        private void handleCollection(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            Type collectionType = propInfo.PropertyType.GenericTypeArguments[0];

            if (checkPrimitiveTypeAndFunction(jsonData, propInfo, collectionType, Func.PrimCollection, handlingMode))
            { /* Done */ }
            else if (collectionType.IsClass && collectionType.IsSubclassOf(typeof(JsonBase)))
            {
                if (handlingMode == JsonHandlingMode.JsonToProperties)
                {
                    MethodInfo methAdd = propInfo.PropertyType.GetMethod("Add");
                    prepareJArrayToCollection(jsonData, propInfo, out object icollection, out JToken[] jArray);

                    for (int i = 0; i < jArray.Length; i++)
                    {
                        JsonBase instance = (JsonBase)Activator.CreateInstance(collectionType);
                        JObject obj = (JObject)jArray[i];
                        instance.handleProperties(obj, handlingMode);
                        methAdd.Invoke(icollection, new object[] { instance });
                    }
                }
                else if (handlingMode == JsonHandlingMode.PropertiesToJson)
                {
                    prepareCollectionToJArray(propInfo, out Array collectionArray, out JArray jArray);

                    for (int i = 0; i < collectionArray.Length; i++)
                        jArray.Add(makeJObjFromJBaseClass(collectionArray.GetValue(i), handlingMode));

                    jsonData[propInfo.Name] = jArray;
                }
            }
        }

        private void handlePrimitiveCollection<T>(JObject jsonData, PropertyInfo propInfo, JsonHandlingMode handlingMode)
        {
            if (handlingMode == JsonHandlingMode.JsonToProperties)
            {
                prepareJArrayToCollection(jsonData, propInfo, out object icollection, out JToken[] jArray);

                MethodInfo methAdd = propInfo.PropertyType.GetMethod("Add");
                for (int i = 0; i < jArray.Length; i++)
                    methAdd.Invoke(icollection, new object[] { (T)jArray[i].ToObject(typeof(T)) });
            }
            else if (handlingMode == JsonHandlingMode.PropertiesToJson)
            {
                prepareCollectionToJArray(propInfo, out Array collectionArray, out JArray jArray);

                for (int i = 0; i < collectionArray.Length; i++)
                    jArray.Add((dynamic)(T)collectionArray.GetValue(i));

                jsonData[propInfo.Name] = jArray;
            }
        }

        private void prepareJArrayToCollection(JObject jsonData, PropertyInfo propInfo, out object collection, out JToken[] jArray)
        {
            collection = Activator.CreateInstance(propInfo.PropertyType);
            propInfo.SetValue(this, collection);
            jArray = jsonData[propInfo.Name].ToArray();
        }

        private void prepareCollectionToJArray(PropertyInfo propInfo, out Array collectionArray, out JArray jArray)
        {
            MethodInfo methToArray = propInfo.PropertyType.GetMethod("ToArray");
            collectionArray = (Array)methToArray.Invoke(propInfo.GetValue(this), null);
            jArray = new JArray();
        }

        private JObject makeJObjFromJBaseClass(object classObject, JsonHandlingMode handlingMode)
        {
            JsonBase cls = (JsonBase)classObject;
            JObject jObj = new JObject();
            cls.handleProperties(jObj, handlingMode); // Recursion
            return jObj;
        }

        private bool checkPrimitiveTypeAndFunction(JObject jsonData, PropertyInfo propInfo, Type type, Func function, JsonHandlingMode handlingMode)
        {
            if (type == typeof(string))
            {
                if (function == Func.PrimType) handlePrimitiveType<string>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<string>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<string>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(bool))
            {
                if (function == Func.PrimType) handlePrimitiveType<bool>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<bool>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<bool>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(Guid))
            {
                if (function == Func.PrimType) handlePrimitiveType<Guid>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<Guid>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<Guid>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(DateTime))
            {
                if (function == Func.PrimType) handlePrimitiveType<DateTime>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<DateTime>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<DateTime>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(TimeSpan))
            {
                if (function == Func.PrimType) handlePrimitiveType<TimeSpan>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<TimeSpan>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<TimeSpan>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(uint))
            {
                if (function == Func.PrimType) handlePrimitiveType<uint>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<uint>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<uint>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(int))
            {
                if (function == Func.PrimType) handlePrimitiveType<int>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<int>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<int>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(double))
            {
                if (function == Func.PrimType) handlePrimitiveType<double>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<double>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<double>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(long))
            {
                if (function == Func.PrimType) handlePrimitiveType<long>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<long>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<long>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(float))
            {
                if (function == Func.PrimType) handlePrimitiveType<float>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<float>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<float>(jsonData, propInfo, handlingMode);
            }
            else if (type == typeof(Uri))
            {
                if (function == Func.PrimType) handlePrimitiveType<Uri>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimArray) handlePrimitiveArray<Uri>(jsonData, propInfo, handlingMode);
                else if (function == Func.PrimCollection) handlePrimitiveCollection<Uri>(jsonData, propInfo, handlingMode);
            }
            else
                return false; // Not a supported Primitive Type

            return true;
        }
    }
}
