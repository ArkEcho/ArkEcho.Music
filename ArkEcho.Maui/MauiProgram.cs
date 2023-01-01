using ArkEcho.Core;
using ArkEcho.RazorPage;

namespace ArkEcho.Maui
{
    public class MauiProgram
    {
        public enum Platform
        {
            Unknown = 0,
            Windows,
            Android,
        }

        public static MauiApp CreateMauiApp(Platform executingPlatform, string rootPath, string musicFolder)
        {
            RazorConfig config = new RazorConfig("ArkEchoMauiConfig.json");

            bool success = false;

            Task.Factory.StartNew(() => success = config.LoadFromFile(rootPath, true).Result).Wait();

            if (!string.IsNullOrEmpty(musicFolder))
                config.MusicFolder = new Uri(musicFolder);

            if (!success)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping");
                return null;
            }

            Rest rest = new Rest(config.ServerAddress, config.Compression);
            if (!rest.CheckConnection())
            {
                Console.WriteLine("### No Response from Server! Maybe its Offline! Stopping");
                return null;
            }

            RestLoggingWorker loggingWorker = new RestLoggingWorker(rest, config.LogLevel);
            loggingWorker.RunWorkerAsync();

            Logger logger = new Logger(Resources.ARKECHOMAUI, "MauiProgram", loggingWorker);

            logger.LogStatic($"Executing on {executingPlatform}, Root Path: {rootPath}");
            logger.LogStatic("Configuration for ArkEcho.Maui:");

            string configString = string.Empty;
            Task.Factory.StartNew(() => configString = config.SaveToJsonString().Result).Wait();

            logger.LogStatic($"\r\n{configString}");

            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>().ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddArkEchoServices<MauiLocalStorage, MauiAppModel>(loggingWorker, config);

            //#if DEBUG
            //            builder.Services.AddBlazorWebViewDeveloperTools();
            //            builder.Logging.AddDebug();
            //#endif

            MauiApp app = builder.Build();

            return app;
        }
    }
}