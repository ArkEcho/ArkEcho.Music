using Android.App;
using Android.OS;

using System.Threading.Tasks;

namespace ArkEcho.App
{
    [Activity(Theme = "@style/ArkEcho.SplashScreen", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Splash);
        }

        protected override async void OnResume()
        {
            base.OnResume();
            
            // Prepare WebSockets Connection
            Websockets.Droid.WebsocketConnection.Link();

            // Bisschen Verzögerung
            await Task.Delay(1500);

            StartActivity(typeof(MainActivity));
        }
    }
}