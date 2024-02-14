using System;
using System.Threading.Tasks;

namespace ArkEcho.Core
{
    public class Authentication
    {
        private Rest rest = null;
        private ILocalStorage localStorage = null;
        private const string SESSIONTOKEN = "AE_ACCESS_TOKEN";

        public User AuthenticatedUser { get; private set; }

        public Authentication(ILocalStorage localStorage, Rest rest)
        {
            this.localStorage = localStorage;
            this.rest = rest;
        }

        public async Task<bool> CheckUserAuthenticated()
        {
            try
            {
                Guid accessToken = await localStorage.GetItemAsync<Guid>(SESSIONTOKEN);
                if (accessToken == Guid.Empty)
                    return false;

                if (!await rest.CheckSession(accessToken))
                {
                    await localStorage.RemoveItemAsync(SESSIONTOKEN);
                    return false;
                }

                AuthenticatedUser = await rest.GetUser(accessToken);
                if (AuthenticatedUser == null)
                    return false;

                await localStorage.SetItemAsync(SESSIONTOKEN, AuthenticatedUser.SessionToken);
                rest.ApiToken = await rest.GetApiToken(AuthenticatedUser.SessionToken);
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public async Task<bool> AuthenticateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            AuthenticatedUser = await rest.AuthenticateUser(username, Encryption.EncryptSHA256(password));

            if (AuthenticatedUser == null)
                return false;

            await localStorage.SetItemAsync(SESSIONTOKEN, AuthenticatedUser.SessionToken);
            rest.ApiToken = await rest.GetApiToken(AuthenticatedUser.SessionToken);
            return true;
        }

        public async Task<bool> LogoutUser()
        {
            try
            {
                Guid accessToken = await localStorage.GetItemAsync<Guid>(SESSIONTOKEN);
                await localStorage.RemoveItemAsync(SESSIONTOKEN);
                await rest.LogoutSession(accessToken);
                AuthenticatedUser = null;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}