using System;

using Android.App;
using Android.OS;
using Android.Widget;

namespace arkecho_app.source.activity
{
    [Activity]
    public class ExtendedActivity : BaseActivity
    {
        Button actionBarMenuButton;
        Button actionBarBackButton;
        PopupMenu actionBarPopupMenu;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Action Bar
            ActionBar.SetDisplayShowCustomEnabled(true);
            ActionBar.SetCustomView(Resource.Layout.ActionBar);

            actionBarBackButton = FindViewById<Button>(Resource.Id.pbActionBarBack);
            actionBarBackButton.Click += onPbActionBarBackClicked;

            actionBarMenuButton = FindViewById<Button>(Resource.Id.pbActionBarMenu);
            actionBarMenuButton.Click += onPbActionBarMenuClicked;

            actionBarPopupMenu = new PopupMenu(this, actionBarMenuButton);
            actionBarPopupMenu.MenuInflater.Inflate(Resource.Menu.top_menu, actionBarPopupMenu.Menu);
            actionBarPopupMenu.MenuItemClick += onPopupMenuItemClicked;
            actionBarPopupMenu.DismissEvent += (s, arg) => { };
        }

        protected void setActionBarTitleText(string title)
        {
            FindViewById<TextView>(Resource.Id.twActionBarTitle).Text = title;
        }

        private void onPbActionBarBackClicked(object sender, EventArgs e)
        {
            base.OnBackPressed();
        }

        private void onPbActionBarMenuClicked(object sender, EventArgs e)
        {
            actionBarPopupMenu.Show();
        }

        private void onPopupMenuItemClicked(object sender, PopupMenu.MenuItemClickEventArgs e)
        {
            int itemId = e.Item.ItemId;
            if (itemId == Resource.Id.top_menu_impressum)
            {
                StartActivity(typeof(ImpressumActivity));
            }
            else if (itemId == Resource.Id.top_menu_help)
            {
                StartActivity(typeof(HelpActivity));
            }
        }

        protected void setActionBarButtonBackHidden(bool hide)
        {
            if (hide) actionBarBackButton.Visibility = Android.Views.ViewStates.Invisible;
            else actionBarBackButton.Visibility = Android.Views.ViewStates.Visible;
        }

        protected void setActionBarButtonMenuHidden(bool hide)
        {
            if (hide) actionBarMenuButton.Visibility = Android.Views.ViewStates.Invisible;
            else actionBarMenuButton.Visibility = Android.Views.ViewStates.Visible;
        }
    }
}