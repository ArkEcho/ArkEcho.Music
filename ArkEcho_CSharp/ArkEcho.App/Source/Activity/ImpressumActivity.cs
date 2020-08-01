using Android.App;
using Android.OS;

namespace arkecho_app.source.activity
{
    [Activity]
    public class ImpressumActivity : ExtendedActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Impressum);

            setActionBarButtonMenuHidden(true);
            setActionBarTitleText(GetString(Resource.String.ImpressumActivityTitle));
        }
    }
}