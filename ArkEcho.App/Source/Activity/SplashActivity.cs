using Android.App;
using Android.OS;

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

            await AppModel.Instance.Init(this);

            StartActivity(typeof(PlayerActivity));
            Finish();
        }
    }
}