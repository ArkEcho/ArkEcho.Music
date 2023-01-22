using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using System.Diagnostics;

namespace ArkEcho.Maui
{
    public class MauiProgram
    {
        public static MauiApp CreateMauiApp(Resources.Platform executingPlatform)
        {
            AppEnvironment environment = new AppEnvironment(Resources.ARKECHOMAUI, Debugger.IsAttached, executingPlatform, true);

            MauiAppBuilder builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>().ConfigureFonts(fonts => { fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); });

            builder.Services.AddMauiBlazorWebView();

#if ANDROID
            builder.Services.AddSingleton<IMauiHelper, AndroidMaui.AndroidMauiHelper>();
#elif WINDOWS
            builder.Services.AddSingleton<IMauiHelper, WinUI.WindowsMauiHelper>();
#endif

            builder.Services.AddArkEchoServices<MauiLocalStorage, MauiAppModel>(environment);

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            MauiApp app = builder.Build();

            return app;
        }
    }
}