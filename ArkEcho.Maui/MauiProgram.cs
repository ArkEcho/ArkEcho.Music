using ArkEcho.Core;
using ArkEcho.Desktop;
using ArkEcho.RazorPage;

namespace ArkEcho.Maui
{
    public static class MauiProgram
    {
        public enum Platform
        {
            Unknown = 0,
            Windows,
            Android,
        }

        public static MauiApp CreateMauiApp(Platform executingPlatform, string rootPath, string musicFolder)
        {
            MauiApp app = null;

            DesktopAppConfig config = new DesktopAppConfig("ArkEchoMauiConfig.json");

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

            RestLoggingWorker LoggingWorker = new RestLoggingWorker(rest, config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            Logger logger = new Logger(Resources.ARKECHOMAUI, "MauiProgram", LoggingWorker);

            logger.LogStatic($"Executing on {executingPlatform}, Root Path: {rootPath}");
            logger.LogStatic("Configuration for ArkEcho.Maui:");

            string configString = string.Empty;
            Task.Factory.StartNew(() => configString = config.SaveToJsonString().Result).Wait();

            logger.LogStatic($"\r\n{configString}");

            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>().ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton(LoggingWorker);
            builder.Services.AddSingleton(config);
            builder.Services.AddScoped<ILocalStorage, DesktopLocalStorage>();
            builder.Services.AddScoped<IAppModel, DesktopAppModel>();

            //#if DEBUG
            //            builder.Services.AddBlazorWebViewDeveloperTools();
            //            builder.Logging.AddDebug();
            //#endif

            app = builder.Build();

            return app;
        }
    }
}