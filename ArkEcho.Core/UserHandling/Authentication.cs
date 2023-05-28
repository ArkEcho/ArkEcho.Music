using ArkEcho.Core;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class Authentication
    {
        private Rest rest = null;
        private ILocalStorage localStorage = null;
        private const string SESSIONTOKEN = "AE_ACCESS_TOKEN";

        public Authentication(ILocalStorage localStorage, Rest rest)
        {
            this.localStorage = localStorage;
            this.rest = rest;
        }

        public async Task<bool> GetAuthenticationState()
        {
            Guid accessToken = Guid.Empty;

            try
            {
                accessToken = await localStorage.GetItemAsync<Guid>(SESSIONTOKEN);
                return await rest.CheckSession(accessToken);
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<User> AuthenticateUserForLogin(string username, string password)
        {
            User authenticated = await rest.AuthenticateUser(username, Encryption.EncryptSHA256(password));

            if (authenticated != null)
                await localStorage.SetItemAsync(SESSIONTOKEN, authenticated.SessionToken);

            return authenticated;
        }

        public async Task<bool> MarkUserAsLoggedOut(Guid sessionToken)
        {
            try
            {
                await localStorage.RemoveItemAsync(SESSIONTOKEN);
                await rest.LogoutSession(sessionToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}