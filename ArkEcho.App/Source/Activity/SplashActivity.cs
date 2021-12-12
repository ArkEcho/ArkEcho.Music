using Android.App;
using Android.Content;
using Android.OS;
using System.Net;

namespace ArkEcho.App
{
    [Activity(Theme = "@style/ArkEcho.SplashScreen", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

            // TODO: Disable on Release Build
            ServicePointManager.ServerCertificateValidationCallback +=  // Disable https Error Fails -> Trust Failure
                    (sender, certificate, chain, sslPolicyErrors) => true;

            SetContentView(Resource.Layout.Splash);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await AppModel.Instance.Init((PowerManager)this.GetSystemService(Context.PowerService));

            // Bisschen Verzögerung
            //await Task.Delay(250);

            StartActivity(typeof(PlayerActivity));
            Finish();
        }
    }
}