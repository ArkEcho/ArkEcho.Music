using Android.App;
using Android.OS;
using Android.Widget;
using ArkEcho.App.Connection;
using ArkEcho.Core;
using System;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    [Activity]
    public class SyncMusicFilesActivity : ExtendedActivity
    {
        Button syncMusicFilesButton = null;
        private ArrayAdapter adapter = null;
        ListView logListView = null;
        private ArkEchoRest arkEchoRest = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SyncMusicFiles);

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);

            logListView = FindViewById<ListView>(Resource.Id.logListView);
            logListView.Adapter = adapter;

            syncMusicFilesButton = FindViewById<Button>(Resource.Id.syncMusicButton);
            syncMusicFilesButton.Click += onSyncMusicFilesButtonClicked;

            setActionBarButtonMenuHidden(true);
            setActionBarTitleText(GetString(Resource.String.SyncMusicFilesActivityTitle));

            arkEchoRest = new ArkEcho.App.Connection.ArkEchoRest();
        }

        private async void onSyncMusicFilesButtonClicked(object sender, EventArgs e)
        {
            string sdCardMusicFolder = ArkEcho.App.AppModel.GetMusicSDFolderPath();

            RestSharp.RestResponse response = null;
            await Task.Run(() => response = (RestSharp.RestResponse)arkEchoRest.GetMusicFileInfo().Result);
            logInListView("", Core.Resources.LogLevel.Error);
        }

        private bool logInListView(string text, Resources.LogLevel level)
        {
            adapter.Add(text);
            adapter.NotifyDataSetChanged();
            return true;
        }
    }
}