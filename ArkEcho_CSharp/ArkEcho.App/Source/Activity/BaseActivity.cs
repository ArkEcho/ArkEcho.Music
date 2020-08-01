using Android.App;
using Android.OS;

namespace arkecho_app.source.activity
{
    [Activity(Label = "@string/ApplicationTitle", Icon = "@drawable/arkecho_logo_small_blue")]
    public class BaseActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }
    }
}