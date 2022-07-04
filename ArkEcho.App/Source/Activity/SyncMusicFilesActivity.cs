using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using ArkEcho.Core;

using System;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    [Activity(ScreenOrientation = ScreenOrientation.Portrait)]
    public class SyncMusicFilesActivity : ExtendedActivity
    {
        Button syncMusicFilesButton = null;
        private ArrayAdapter adapter = null;
        ListView logListView = null;

        Logger logger = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SyncMusicFiles);

            adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1);

            logListView = FindViewById<ListView>(Resource.Id.syncLogListView);
            logListView.Adapter = adapter;

            syncMusicFilesButton = FindViewById<Button>(Resource.Id.syncMusicButton);
            syncMusicFilesButton.Click += onSyncMusicFilesButtonClicked;

            setActionBarButtonMenuHidden(true);
            setActionBarTitleText(GetString(Resource.String.SyncMusicFilesActivityTitle));

            logger = new Logger(ArkEcho.Resources.ARKECHOAPP, "SyncActivity", AppModel.Instance.RestLoggingWorker);
        }

        private async void onSyncMusicFilesButtonClicked(object sender, EventArgs e)
        {
            // TODO: Playlists laden
            AppModel.Instance.PreventLock();

            logger.LogStatic($"Starting Music Sync!");

            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Starting");
            adapter.NotifyDataSetChanged();
            await Task.Delay(200);

            logger.LogImportant("Loading MusicLibrary");

            bool loadlib = await AppModel.Instance.LoadLibraryFromServer();
            if (!loadlib)
            {
                AppModel.Instance.AllowLock();
                return;
            }

            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Done loading library");
            adapter.NotifyDataSetChanged();
            await Task.Delay(200);

            //if (!checkLib)
            //{
            //    AppModel.Instance.AllowLock();
            //    return;
            //}

            //adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Checked local Folder, {missing.Count} missing. Loading...");
            //adapter.NotifyDataSetChanged();
            //await Task.Delay(200);



            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Success");
            adapter.NotifyDataSetChanged();
            await Task.Delay(1000);

            AppModel.Instance.AllowLock();
        }
    }
}