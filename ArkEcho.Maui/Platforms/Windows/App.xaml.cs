using Microsoft.Maui.Handlers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;

namespace ArkEcho.Maui.WinUI;

public partial class App : MauiWinUIApplication
{
    AppWindow appWindow = null;
    public App()
    {
        this.InitializeComponent();

        WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();

            // Disable Default TitleBar -> was behind actual Close/Min/Max Buttons
            nativeWindow.ExtendsContentIntoTitleBar = false;

            nativeWindow.SizeChanged += (object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args) => setDragRegion();
            nativeWindow.Title = "TestBlazorMaui";

            var hWnd = WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 39, 39, 47);
            appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 50, 51, 61);
            appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 39, 39, 47);

            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            setDragRegion();
        });
    }

    private void setDragRegion()
    {
        RectInt32 rect = new RectInt32
        {
            X = appWindow.TitleBar.LeftInset,
            Y = 0,
            Height = appWindow.TitleBar.Height,
            Width = appWindow.Size.Width - appWindow.TitleBar.RightInset
        };

        appWindow.TitleBar.SetDragRectangles(new RectInt32[] { rect });
    }

    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp(ArkEcho.Resources.Platform.Windows);
        if (app == null)
            Exit();
        return app;
    }
}