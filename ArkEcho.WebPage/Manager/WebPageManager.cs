using ArkEcho.RazorPage;
using ArkEcho.RazorPage.Data;
using ArkEcho.WebPage.Data;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ArkEcho.WebPage
{
    // TODO: Mehr Logging, besonders im Player
    // TODO: Alle Manager und Init nach AppModel führen!
    public class WebPageManager : IDisposable
    {
        private WebAssemblyHostBuilder builder = null;

        public static WebPageManager Instance { get; private set; } = new WebPageManager();

        public bool Initialized { get; private set; } = false;

        private WebPageManager()
        {
        }

        public async Task<bool> Init()
        {
            if (Initialized)
                return Initialized;

            builder = WebAssemblyHostBuilder.CreateDefault();

            AppEnvironment environment = new AppEnvironment(Resources.ARKECHOWEBPAGE, builder.HostEnvironment.IsDevelopment(), Resources.Platform.Web, false);

            builder.RootComponents.Add<ArkEchoApp>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorageAsSingleton(); // For WebLocalStorage
            builder.Services.AddArkEchoServices<WebLocalStorage, WebAppModel>(environment);

            Initialized = true;
            return Initialized;
        }

        public async Task Start()
        {
            await builder.Build().RunAsync();
        }

        #region Dispose

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
