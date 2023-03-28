// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.Maui.Handlers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using WinRT.Interop;

namespace ArkEcho.Maui.WinUI;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
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

            // The TitleBar is null when not Windows 11 (well...at least on Windows 10 machines) 
            if (appWindow.TitleBar != null)
            {
                appWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 39, 39, 47);
                appWindow.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Color.FromArgb(255, 50, 51, 61);
                appWindow.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Color.FromArgb(255, 39, 39, 47);

                appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
                appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

                setDragRegion();
            }
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