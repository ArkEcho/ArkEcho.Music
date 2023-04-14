using ArkEcho.Core;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace ArkEcho.Maui.WinUI
{
    public class WindowsMauiHelper : IMauiHelper
    {
        private Microsoft.UI.Xaml.Window nativeWindow = null;
        private AppWindow appWindow = null;
        private bool fullTitleBar = true;

        public string GetPlatformSpecificMusicFolder(User user)
        {
            UserSettings.UserPath path = user.Settings.GetLocalUserSettings();
            if (path != null)
                return path.Path.LocalPath;
            else
                return string.Empty;
        }

        public async Task<string> PickFolder()
        {
            var folderPicker = new FolderPicker();
            // Make it work for Windows 10
            folderPicker.FileTypeFilter.Add("*");
            // Get the current window's HWND by passing in the Window object
            var hwnd = ((MauiWinUIWindow)App.Current.Application.Windows[0].Handler.PlatformView).WindowHandle;

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

            var result = await folderPicker.PickSingleFolderAsync();

            if (result == null)
                return string.Empty;

            return result.Path;
        }

        public void SetWindow(Microsoft.UI.Xaml.Window nativeMauiWindow, bool darkMode)
        {
            nativeWindow = nativeMauiWindow;
            nativeWindow.Activate();

            var hWnd = WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            appWindow = AppWindow.GetFromWindowId(windowId);

            // Disable Default TitleBar -> was behind actual Close/Min/Max Buttons
            nativeWindow.ExtendsContentIntoTitleBar = false;
            nativeWindow.SizeChanged += (object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args) => setDragRegion();

            Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfoChanged += (object sender, DisplayInfoChangedEventArgs args) => setDragRegion();

            appWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            appWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

            ApplicationTitle = "ArkEcho Music";
            DarkMode = darkMode;
        }

        public string ApplicationTitle
        {
            get { return nativeWindow.Title; }
            set { nativeWindow.Title = value; }
        }

        public bool DarkMode
        {
            set { setButtonColor(value); }
        }

        public void SetDragArea(bool fullTitlebar)
        {
            this.fullTitleBar = fullTitlebar;
            setDragRegion();
        }

        private void setButtonColor(bool darkMode)
        {
            // TODO: LightMode Buttons
            Windows.UI.Color backgroundColor = Windows.UI.Color.FromArgb(255, 39, 39, 47);
            Windows.UI.Color hoverBackgroundColor = Windows.UI.Color.FromArgb(255, 50, 51, 61);

            appWindow.TitleBar.ButtonBackgroundColor = backgroundColor;
            appWindow.TitleBar.ButtonInactiveBackgroundColor = backgroundColor;
            appWindow.TitleBar.ButtonHoverBackgroundColor = hoverBackgroundColor;
        }

        private int getTitleBarHeight()
        {
            double displayHeight = Microsoft.Maui.Devices.DeviceDisplay.MainDisplayInfo.Height;
            if (displayHeight >= 2160) return 96;// 4k
            else if (displayHeight >= 1440) return 72;// 1440p
            else return 48; // 1080p
        }

        private void setDragRegion()
        {
            int titleBarHeight = getTitleBarHeight();
            int navbuttonWidth = fullTitleBar ? 0 : Convert.ToInt32(titleBarHeight * 0.85); // Left Navigation Menu
            int avatarButtonWidth = fullTitleBar ? 0 : Convert.ToInt32(titleBarHeight * 1.5); // Right Avatar Button
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
    }
}
