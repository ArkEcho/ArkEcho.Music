using ArkEcho.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace ArkEcho.WebPage
{
    public class WebPageManager : IDisposable
    {
        private const string webPageConfigFileName = "WebPageConfig.json";

        private IWebHost host = null;
        private Logger logger = null;
        private MusicLibrary library = null;
        private Rest rest = null;

        public static WebPageManager Instance { get; private set; } = new WebPageManager();

        public RestLoggingWorker LoggingWorker { get; private set; } = null;

        public WebPageConfig Config { get; private set; }

        public bool Initialized { get; private set; } = false;

        private WebPageManager()
        {
            library = new MusicLibrary();
        }

        public bool Init()
        {
            if (Initialized)
                return Initialized;

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            Config = new WebPageConfig(webPageConfigFileName);
            if (!Config.LoadFromFile(executingLocation, true).Result)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping WebPage");
                return false;
            }

            rest = new Rest(Config.ServerAddress, Config.Compression);

            LoggingWorker = new RestLoggingWorker(rest, (Logging.LogLevel)Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new Logger("WebPage", "Manager", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.WebPage:");
            logger.LogStatic($"\r\n{Config.SaveToJsonString().Result}");

            // TODO: Singleton und Port per Config
            host = WebHost.CreateDefaultBuilder()
                               .UseUrls($"https://*:{Config.Port}")
                               .UseKestrel()
                               .UseStartup<Startup>()
                               .Build();

            Initialized = true;
            return Initialized;
        }

        public void Start()
        {
            host.Run();
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
