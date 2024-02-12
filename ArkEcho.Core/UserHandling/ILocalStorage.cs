using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public interface ILocalStorage
    {
        Task<T> GetItemAsync<T>(string key);

        Task RemoveItemAsync(string key);

        Task SetItemAsync<T>(string key, T data);
    }

    public abstract class LocalStorageBase : ILocalStorage
    {
        public abstract Task<T> GetItemAsync<T>(string key);
        public abstract Task RemoveItemAsync(string key);
        public abstract Task SetItemAsync<T>(string key, T data);
    }
}
