using ArkEcho.Core;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Reflection;

namespace ArkEcho.WebPage
{
    // TODO: Mehr Logging, besonders im Player
    public class WebPageManager : IDisposable
    {
        private const string webPageConfigFileName = "WebPageConfig.json";

        private IWebHost host = null;
        private Logger logger = null;
        private Rest rest = null;

        public static WebPageManager Instance { get; private set; } = new WebPageManager();

        public RestLoggingWorker LoggingWorker { get; private set; } = null;

        public WebPageConfig Config { get; private set; }

        public bool Initialized { get; private set; } = false;

        private WebPageManager()
        {
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
            if (!rest.CheckConnection())
            {
                Console.WriteLine("### No Response from Server! Maybe its Offline! Stopping WebPage");
                return false;
            }

            LoggingWorker = new RestLoggingWorker(rest, Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            logger = new Logger(Resources.ARKECHOWEBPAGE, "Manager", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.WebPage:");
            logger.LogStatic($"\r\n{Config.SaveToJsonString().Result}");

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
