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
            void lockScreen()
            {
                AppModel.Instance.PreventLock();
            }
            async Task endLock()
            {
                await Task.Delay(1000);
                AppModel.Instance.AllowLock();
            }

            lockScreen();

            logger.LogStatic($"Starting Music Sync!");

            await showProgress("Starting");

            logger.LogImportant("Loading MusicLibrary");

            bool loadlib = await AppModel.Instance.LoadLibraryFromServer();
            if (!loadlib)
            {
                await endLock();
                return;
            }

            await showProgress("Done loading Library... Sync and CleanUp!");

            bool syncMusicFiles = await AppModel.Instance.SyncMusicFiles();
            if (!syncMusicFiles)
            {
                await endLock();
                return;
            }

            await showProgress("Success");

            await endLock();
        }

        private async Task showProgress(string text)
        {
            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: {text}");
            adapter.NotifyDataSetChanged();
            await Task.Delay(100);
        }
    }
}