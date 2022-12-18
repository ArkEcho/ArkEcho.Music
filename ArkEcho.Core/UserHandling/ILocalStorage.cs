using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public interface ILocalStorage
    {
        Task<T> GetItemAsync<T>(string key);

        Task RemoveItemAsync(string key);

        Task SetItemAsync<T>(string key, T data);
    }
}
