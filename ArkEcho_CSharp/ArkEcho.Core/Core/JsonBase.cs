using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace ArkEcho.Core
{
    public abstract class JsonBase
    {
        protected class JsonProperty : Attribute
        {
            // TODO: Rausschmeissen? Nur bei Config sinnvoll
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

        private void LoadJsonFromJObjectAndSetProperties(JObject Object)
        {
            data = Object;
            loadProperties();
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
                        else if (info.PropertyType == typeof(DateTime))
                            checkKeyAndSetProperty<DateTime>(attribute, info);
                        else if (info.PropertyType == typeof(TimeSpan))
                            checkKeyAndSetProperty<TimeSpan>(attribute, info);
                        else if (info.PropertyType == typeof(uint))
                            checkKeyAndSetProperty<uint>(attribute, info);
                        else if (info.PropertyType == typeof(int))
                            checkKeyAndSetProperty<int>(attribute, info);
                        else if (info.PropertyType == typeof(double))
                            checkKeyAndSetProperty<double>(attribute, info);
                        else if (info.PropertyType == typeof(long))
                            checkKeyAndSetProperty<long>(attribute, info);
                        else if (info.PropertyType == typeof(float))
                            checkKeyAndSetProperty<float>(attribute, info);
                        else if (info.PropertyType.IsClass)
                        {
                            if (info.PropertyType.IsSubclassOf(typeof(JsonBase)))
                            {
                                JsonBase instance = (JsonBase)Activator.CreateInstance(info.PropertyType);
                                info.SetValue(this, instance);

                                if (!data.ContainsKey(info.Name))
                                    data[info.Name] = new JObject();
                                JObject obj = (JObject)data[info.Name];

                                instance.LoadJsonFromJObjectAndSetProperties(obj);
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
