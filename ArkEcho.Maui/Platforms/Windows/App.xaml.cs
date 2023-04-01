using Microsoft.Maui.Handlers;

namespace ArkEcho.Maui.WinUI;

public partial class App : MauiWinUIApplication
{
    private WindowsMauiHelper helper = null;

    public App()
    {
        this.InitializeComponent();
        helper = new WindowsMauiHelper();

        WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            // TODO: DarkMode from Windows Settings
            helper.SetWindow(handler.PlatformView, true);
        });
    }

    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp(ArkEcho.Resources.Platform.Windows, helper);
        if (app == null)
            Exit();
        return app;
    }
}