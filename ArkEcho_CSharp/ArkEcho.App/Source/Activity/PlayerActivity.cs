using Android.App;
using Android.OS;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    [Activity]
    public class PlayerActivity : ExtendedActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Player);

            setActionBarButtonBackHidden(true);
            setActionBarTitleText(GetString(Resource.String.PlayerActivityTitle));

            FindViewById<Button>(Resource.Id.pbPlay_Pause).Click += onPbPlay_PauseClicked;
            FindViewById<Button>(Resource.Id.pbForward).Click += onPbForwardClicked;
            FindViewById<Button>(Resource.Id.pbBackward).Click += onPbBackwardClicked;
        }

        private void onPbBackwardClicked(object sender, EventArgs e)
        {
        }

        private void onPbForwardClicked(object sender, EventArgs e)
        {
        }

        private void onPbPlay_PauseClicked(object sender, EventArgs e)
        {
        }

        public override async void OnBackPressed()
        {
            await Task.Delay(10);

            Finish();
            //RunOnUiThread(() => base.OnBackPressed());
        }
    }
}