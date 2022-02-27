using ArkEcho.Core;
using Blazored.LocalStorage;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class Authentication
    {
        public ILocalStorageService localStorageService { get; }

        public AppModel model { get; }

        public Authentication(ILocalStorageService localStorageService, AppModel model)
        {
            this.localStorageService = localStorageService;
            this.model = model;
        }

        public async Task<bool> GetAuthenticationState()
        {
            Guid accessToken = Guid.Empty;

            try
            {
                accessToken = await localStorageService.GetItemAsync<Guid>("accessToken");
                //Console.WriteLine($"Access Token from LocalStorage: {accessToken}");

                User user = await model.Rest.CheckUserToken(accessToken);

                if (user != null)
                {
                    //Console.WriteLine($"User Checked OK");
                    return true;
                }
            }
            catch (Exception ex)
            {
                // TODO: On first render Exception because js cant work?!
            }
            return false;
        }

        public async Task<User> AuthenticateUserForLogin(User user)
        {
            return await model.Rest.AuthenticateUserForLogin(user);
        }

        public async Task MarkUserAsAuthenticated(User user)
        {
            await localStorageService.SetItemAsync("accessToken", user.AccessToken);
            //Console.WriteLine($"Added Token To LocalStorage: {user.AccessToken}");
        }

        public async Task MarkUserAsLoggedOut()
        {
            try
            {
                await localStorageService.RemoveItemAsync("accessToken");
                //Console.WriteLine($"Removed Token From LocalStorage");
            }
            catch (Exception)
            {
            }
        }
    }
}