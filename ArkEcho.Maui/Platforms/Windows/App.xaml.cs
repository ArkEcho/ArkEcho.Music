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

            Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfoChanged += (object sender, DisplayInfoChangedEventArgs args) => setDragRegion();

            var hWnd = WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            setButtonColor();
        });
    }

    private void setButtonColor()
    {
        Windows.UI.Color backgroundColor = Windows.UI.Color.FromArgb(255, 55, 55, 64);
        Windows.UI.Color hoverBackgroundColor = Windows.UI.Color.FromArgb(255, 50, 51, 61);

        appWindow.TitleBar.ButtonBackgroundColor = backgroundColor;
        appWindow.TitleBar.ButtonInactiveBackgroundColor = backgroundColor;
        appWindow.TitleBar.ButtonHoverBackgroundColor = hoverBackgroundColor;
    }

    private int getTitleBarHeight()
    {
        double displayHeight = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Height;
        if (displayHeight >= 2160) // 4k
            return 96;
        else if (displayHeight >= 1440) // 1440p
            return 72;
        else
            return 48;
    }

    private void setDragRegion()
    {
        int titleBarHeight = getTitleBarHeight();
        int navbuttonWidth = Convert.ToInt32(titleBarHeight * 0.85); // Left Navigation Menu
        int avatarButtonWidth = Convert.ToInt32(titleBarHeight * 1.5); // Right Avatar Button
        int systemButtonWidth = titleBarHeight * 3; // Min/Max/Close

        RectInt32 rect = new RectInt32
        {
            X = navbuttonWidth,
            Y = 0,
            Height = titleBarHeight,
            Width = ((appWindow.Size.Width - (systemButtonWidth)) - avatarButtonWidth) - navbuttonWidth
        };

        appWindow.TitleBar.SetDragRectangles(new RectInt32[] { rect });
    }

    protected override MauiApp CreateMauiApp()
    {
        var app = MauiProgram.CreateMauiApp(ArkEcho.Resources.Platform.Windows, new WindowsMauiHelper());
        if (app == null)
            Exit();
        return app;
    }
}