using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public ILocalStorageService _localStorageService { get; }

        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Guid accessToken = Guid.Empty;
            ClaimsIdentity identity = new ClaimsIdentity();

            try
            {
                accessToken = await _localStorageService.GetItemAsync<Guid>("accessToken");
                User user = Server.ArkEchoServer.Instance.CheckUserToken(accessToken);
                if (user != null)
                    identity = GetClaimsIdentity(user);
            }
            catch (Exception ex)
            {
                // TODO: On first render Exception because js cant work?!
            }

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task MarkUserAsAuthenticated(User user)
        {
            await _localStorageService.SetItemAsync("accessToken", user.AccessToken);

            ClaimsIdentity identity = GetClaimsIdentity(user);

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            try
            {
                await _localStorageService.RemoveItemAsync("accessToken");
            }
            catch (Exception)
            {
            }

            ClaimsIdentity identity = new ClaimsIdentity();

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            if (user.UserName != null)
                claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.UserName), }, "apiauth_type");

            return claimsIdentity;
        }
    }
}