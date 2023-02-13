using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using System.Diagnostics;

#if WINDOWS
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
#endif

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

            // FullScreen
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windowsLifecycleBuilder =>
                {
                    windowsLifecycleBuilder.OnWindowCreated(window =>
                    {
                        window.ExtendsContentIntoTitleBar = false;
                        var handle = WinRT.Interop.WindowNative.GetWindowHandle(window);
                        var id = Win32Interop.GetWindowIdFromWindow(handle);
                        var appWindow = AppWindow.GetFromWindowId(id);
                        switch (appWindow.Presenter)
                        {
                            case OverlappedPresenter overlappedPresenter:
                                //overlappedPresenter.SetBorderAndTitleBar(false, false); // Without Task and Window Bar!
                                overlappedPresenter.Maximize();
                                break;
                        }
                    });
                });
            });
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