using ArkEcho;
using ArkEcho.Core;
using ArkEcho.RazorPage;
using ArkEcho.RazorPage.Data;
using ArkEcho.WebPage;
using ArkEcho.WebPage.Data;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

AppEnvironment environment = new AppEnvironment(Resources.ARKECHOWEBPAGE, builder.HostEnvironment.IsDevelopment(), Resources.Platform.Web, false);

builder.RootComponents.Add<ArkEchoApp>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorageAsSingleton(); // For WebLocalStorage
builder.Services.AddSingleton<BrowserCloseActionsController>();
builder.Services.AddArkEchoServices<WebLocalStorage, WebAppModel, JSPlayer>(environment);

await builder.Build().RunAsync();