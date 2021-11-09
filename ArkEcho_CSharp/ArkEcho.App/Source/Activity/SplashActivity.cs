using Android.App;
using Android.OS;
using System.Net;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    [Activity(Theme = "@style/ArkEcho.SplashScreen", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

#if DEBUG
            ServicePointManager.ServerCertificateValidationCallback +=  // Disable https Error Fails -> Trust Failure
                    (sender, certificate, chain, sslPolicyErrors) => true;
#endif

            SetContentView(Resource.Layout.Splash);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            await AppModel.Instance.Init();

            // Bisschen Verzögerung
            await Task.Delay(500);

            StartActivity(typeof(PlayerActivity));
            Finish();
        }
    }
}