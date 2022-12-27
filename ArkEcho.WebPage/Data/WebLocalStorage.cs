using ArkEcho.Core;
using Blazored.LocalStorage;
using System.Threading.Tasks;

namespace ArkEcho.WebPage.Data
{
    public class WebLocalStorage : LocalStorageBase
    {
        private ILocalStorageService blazorLocalStorage = null;

        public WebLocalStorage(ILocalStorageService blazorLocalStorage)
        {
            this.blazorLocalStorage = blazorLocalStorage;
        }

        public override async Task<T> GetItemAsync<T>(string key)
        {
            return await blazorLocalStorage.GetItemAsync<T>(key);
        }

        public override async Task RemoveItemAsync(string key)
        {
            await blazorLocalStorage.RemoveItemAsync(key);
        }

        public override async Task SetItemAsync<T>(string key, T data)
        {
            await blazorLocalStorage.SetItemAsync(key, data);
        }
    }
}
