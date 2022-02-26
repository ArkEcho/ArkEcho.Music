using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class Program
    {
        /* TODO
	 * Anzeige
			-> Interpreten A-Z -> Doppel & dreifach Interpreten zusammenfassen
			-> Album A-Z
			-> Titel A-Z
			-> Ordner	-> Baumstruktur, ganzen Ordner abspielen
	*/
        public static async Task Main(string[] args)
        {
            WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            // Authentication & Authorization
            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

            // Local Storage
            builder.Services.AddBlazoredLocalStorage();

            builder.Services.AddScoped(sp => new AppModel());
            builder.Services.AddScoped(sp => new ArkEchoJSPlayer());

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}
