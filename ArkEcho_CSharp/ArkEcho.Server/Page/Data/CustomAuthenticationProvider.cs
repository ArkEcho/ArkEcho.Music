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
            try
            {
                accessToken = await _localStorageService.GetItemAsync<Guid>("accessToken");
            }
            catch (Exception ex)
            {
                // TODO: On first render Exception because js cant work?!
            }

            ClaimsIdentity identity = null;

            User user = Server.ArkEchoServer.Instance.Users.Find(x => x.AccessToken.Equals(accessToken));
            if (user != null)
                identity = GetClaimsIdentity(user);
            else
                identity = new ClaimsIdentity();

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task MarkUserAsAuthenticated(User user)
        {
            await _localStorageService.SetItemAsync("accessToken", user.AccessToken);

            var identity = GetClaimsIdentity(user);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("accessToken");

            ClaimsIdentity identity = new ClaimsIdentity();

            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            if (user.EmailAddress != null)
                claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.EmailAddress), }, "apiauth_type");

            return claimsIdentity;
        }
    }
}