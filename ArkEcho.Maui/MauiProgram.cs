using ArkEcho.Core;
using ArkEcho.Desktop;
using ArkEcho.RazorPage;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace ArkEcho.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // TODO: Nicht null return!
            MauiApp app = null;

            DesktopAppConfig Config = new DesktopAppConfig("ArkEchoDesktopConfig.json");

            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            bool success = false;

            Task.Factory.StartNew(() => success = Config.LoadFromFile(executingLocation, true).Result).Wait();

            if (!success)
            {
                Console.WriteLine("### No Config File found/Error Loading -> created new one, please configure. Stopping Desktop");
                return null;
            }

            Rest rest = new Rest(Config.ServerAddress, Config.Compression);
            if (!rest.CheckConnection())
            {
                Console.WriteLine("### No Response from Server! Maybe its Offline! Stopping Desktop");
                return null;
            }

            RestLoggingWorker LoggingWorker = new RestLoggingWorker(rest, Config.LogLevel);
            LoggingWorker.RunWorkerAsync();

            Logger logger = new Logger(Resources.ARKECHODESKTOP, "Form", LoggingWorker);

            logger.LogStatic("Configuration for ArkEcho.Desktop:");

            string configString = string.Empty;
            Task.Factory.StartNew(() => configString = Config.SaveToJsonString().Result).Wait();

            logger.LogStatic($"\r\n{configString}");

            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>().ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddSingleton(LoggingWorker);
            builder.Services.AddSingleton(Config);
            builder.Services.AddScoped<ILocalStorage, DesktopLocalStorage>();
            builder.Services.AddScoped<IAppModel, DesktopAppModel>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            app = builder.Build();

            return app;
        }
    }
}