using ArkEcho.Core;
using ArkEcho.RazorPage;
using ArkEcho.RazorPage.Data;
using ArkEcho.WebPage.Data;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Reflection;

namespace ArkEcho.WebPage
{
    // TODO: Mehr Logging, besonders im Player
    // TODO: Alle Manager und Init nach AppModel führen!
    public class WebPageManager : IDisposable
    {
        private const string webPageConfigFileName = "WebPageConfig.json";

        private WebAssemblyHostBuilder builder = null;
        private Logger logger = null;
        private Rest rest = null;

        public static WebPageManager Instance { get; private set; } = new WebPageManager();

        public RestLoggingWorker LoggingWorker { get; private set; } = null;

        public WebPageConfig Config { get; private set; }

        public bool Initialized { get; private set; } = false;

        private WebPageManager()
        {
        }

        public async Task<bool> Init()
        {
            if (Initialized)
                return Initialized;

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            WebPageConfig Config = new WebPageConfig(webPageConfigFileName);

            rest = new Rest(Config.ServerAddress, false, Config.Compression);

            Console.WriteLine($"BEFORE!");

            if (!await rest.CheckConnection())
            {
                Console.WriteLine("### No Response from Server! Maybe its Offline! Stopping WebPage");
                return false;
            }

            Console.WriteLine($"AFTER!");

            LoggingWorker = new RestLoggingWorker(rest, Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new Logger(Resources.ARKECHOWEBPAGE, "Manager", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.WebPage:");
            logger.LogStatic($"\r\n{await Config.SaveToJsonString()}");

            builder = WebAssemblyHostBuilder.CreateDefault();

            builder.RootComponents.Add<ArkEchoApp>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            builder.Services.AddBlazoredLocalStorageAsSingleton(); // For WebLocalStorage
            builder.Services.AddArkEchoServices<WebLocalStorage, WebAppModel>(rest, LoggingWorker, Config);

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
