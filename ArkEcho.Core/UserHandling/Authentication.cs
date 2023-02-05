using ArkEcho.Core;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class Authentication
    {
        private Rest rest = null;
        private ILocalStorage localStorage = null;
        private const string ACCESSTOKEN = "AE_ACCESS_TOKEN";

        public Authentication(ILocalStorage localStorage, Rest rest)
        {
            this.localStorage = localStorage;
            this.rest = rest;
        }

        public async Task<User> GetAuthenticationState()
        {
            Guid accessToken = Guid.Empty;

            try
            {
                accessToken = await localStorage.GetItemAsync<Guid>(ACCESSTOKEN);

                User authenticated = await rest.CheckUserToken(accessToken);

                return authenticated;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

        public async Task<User> AuthenticateUserForLogin(string username, string password)
        {
            User authenticated = await rest.AuthenticateUserForLogin(new User() { UserName = username, Password = password });

            if (authenticated != null)
                await localStorage.SetItemAsync(ACCESSTOKEN, authenticated.AccessToken);

            return authenticated;
        }

        public async Task<bool> MarkUserAsLoggedOut(Guid accessToken)
        {
            try
            {
                await rest.LogoutUser(accessToken);
                await localStorage.RemoveItemAsync(ACCESSTOKEN);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}