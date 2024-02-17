using Android.App;
using Android.Runtime;

namespace ArkEcho.Maui.AndroidMaui
{
#if DEBUG                                   
    [Application(UsesCleartextTraffic = true)]
#else                                       
[Application]                               
#endif

    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp(ArkEcho.Resources.Platform.Android, new AndroidMauiHelper());
        }
    }
}