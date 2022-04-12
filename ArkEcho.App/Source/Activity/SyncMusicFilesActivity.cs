using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using ArkEcho.Core;

using System;
using System.Collections.Generic;
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

            logger = new Logger($"App", "SyncActivity", AppModel.Instance.RestLoggingWorker);
        }

        private async void onSyncMusicFilesButtonClicked(object sender, EventArgs e)
        {
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

            logger.LogImportant($"Checking Files");

            List<MusicFile> exist = new List<MusicFile>();
            List<MusicFile> missing = new List<MusicFile>();
            bool checkLib = await AppModel.Instance.CheckLibraryWithLocalFolder(exist, missing);

            if (!checkLib)
            {
                AppModel.Instance.AllowLock();
                return;
            }

            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Checked local Folder, {missing.Count} missing. Loading...");
            adapter.NotifyDataSetChanged();
            await Task.Delay(200);

            if (missing.Count > 0)
            {
                logger.LogImportant($"Loading {missing.Count} Files");

                try
                {
                    foreach (MusicFile file in missing)
                    {
                        logger.LogDebug($"Loading {file.FileName}");

                        bool success = await AppModel.Instance.LoadFileFromServer(file);
                        if (!success)
                        {
                            logger.LogError($"Error loading {file.FileName} from Server!");
                            //AppModel.Instance.AllowLock();
                            //return;
                        }

                        exist.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"Exception loading MusicFiles: {ex.Message}");
                    AppModel.Instance.AllowLock();
                    return;
                }
            }

            logger.LogImportant($"Cleaning Up");

            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Cleaning up!");
            adapter.NotifyDataSetChanged();
            await Task.Delay(200);

            await AppModel.Instance.CleanUpFolder(AppModel.GetAndroidMediaAppSDFolderPath(), exist);

            logger.LogStatic($"Success!");

            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: Success");
            adapter.NotifyDataSetChanged();
            await Task.Delay(1000);

            AppModel.Instance.AllowLock();
        }
    }
}