using ArkEcho.Core;

namespace ArkEcho.Maui
{
    public class MauiLocalStorage : LocalStorageBase
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();

        public override async Task<T> GetItemAsync<T>(string key)
        {
            if (storage.TryGetValue(key, out var value))
                return (T)value;
            else
                return default(T);
        }

        public override async Task RemoveItemAsync(string key)
        {
            storage.Remove(key);
        }

        public override async Task SetItemAsync<T>(string key, T data)
        {
            storage.Add(key, data);
        }
    }
}
