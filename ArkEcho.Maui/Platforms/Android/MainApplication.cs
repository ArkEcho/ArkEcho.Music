using Android.App;
using Android.Runtime;

namespace ArkEcho.Maui.AndroidMaui
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp(ArkEcho.Resources.Platform.Android);
        }
    }
}