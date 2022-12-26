using ArkEcho.Core;

namespace ArkEcho.Maui
{
    public class MauiLocalStorage : ILocalStorage
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();

        public async Task<T> GetItemAsync<T>(string key)
        {
            if (storage.TryGetValue(key, out var value))
                return (T)value;
            else
                return default(T);
        }

        public async Task RemoveItemAsync(string key)
        {
            storage.Remove(key);
        }

        public async Task SetItemAsync<T>(string key, T data)
        {
            storage.Add(key, data);
        }
    }
}
