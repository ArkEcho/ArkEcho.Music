using Android.App;
using Android.OS;

namespace arkecho_app.source.activity
{
    [Activity]
    public class HelpActivity : ExtendedActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Help);

            setActionBarButtonMenuHidden(true);
            setActionBarTitleText(GetString(Resource.String.HelpActivityTitle));
        }
    }
}