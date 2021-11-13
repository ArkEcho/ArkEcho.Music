using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ArkEcho.Server
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        public ILocalStorageService _localStorageService { get; }

        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            //throw new Exception("CustomAuthenticationStateProviderException");
            _localStorageService = localStorageService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var accessToken = await _localStorageService.GetItemAsync<string>("accessToken");

            ClaimsIdentity identity = null;

            if (!string.IsNullOrEmpty(accessToken))
            {
                User user = Server.ArkEchoServer.Instance.Users.Find(x => x.AccessToken.Equals(accessToken, System.StringComparison.OrdinalIgnoreCase));
                identity = GetClaimsIdentity(user);
            }
            else
                identity = new ClaimsIdentity();

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        public async Task MarkUserAsAuthenticated(User user)
        {
            await _localStorageService.SetItemAsync("accessToken", user.AccessToken);
            await _localStorageService.SetItemAsync("refreshToken", user.RefreshToken);

            var identity = GetClaimsIdentity(user);

            var claimsPrincipal = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task MarkUserAsLoggedOut()
        {
            await _localStorageService.RemoveItemAsync("refreshToken");
            await _localStorageService.RemoveItemAsync("accessToken");

            ClaimsIdentity identity = new ClaimsIdentity();

            ClaimsPrincipal user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            if (user.EmailAddress != null)
            {
                claimsIdentity = new ClaimsIdentity(new[]
                                {
                                    new Claim(ClaimTypes.Name, user.EmailAddress),
                                    new Claim(ClaimTypes.Role, user.Role.RoleDesc),
                                }, "apiauth_type");
            }

            return claimsIdentity;
        }
    }
}