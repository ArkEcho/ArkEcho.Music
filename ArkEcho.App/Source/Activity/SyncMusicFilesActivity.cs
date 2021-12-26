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
        }

        private async void onSyncMusicFilesButtonClicked(object sender, EventArgs e)
        {
            AppModel.Instance.PreventLock();

            logInListView("Loading MusicLibrary", ArkEcho.Resources.LogLevel.Information);

            bool loadlib = await AppModel.Instance.LoadLibraryFromServer(logInListView);
            if (!loadlib)
            {
                AppModel.Instance.AllowLock();
                return;
            }

            logInListView($"Checking Files", ArkEcho.Resources.LogLevel.Information);
            await Task.Delay(200);

            List<MusicFile> exist = new List<MusicFile>();
            List<MusicFile> missing = new List<MusicFile>();
            bool checkLib = await AppModel.Instance.CheckLibraryWithLocalFolder(logInListView, exist, missing);

            if (!checkLib)
            {
                AppModel.Instance.AllowLock();
                return;
            }

            if (missing.Count > 0)
            {
                logInListView($"Loading {missing.Count} Files", ArkEcho.Resources.LogLevel.Information);
                await Task.Delay(200);

                try
                {
                    foreach (MusicFile file in missing)
                    {
                        logInListView($"Loading {file.FileName}", ArkEcho.Resources.LogLevel.Information);

                        bool success = await AppModel.Instance.LoadFileFromServer(file, logInListView);
                        if (!success)
                        {
                            logInListView($"Error loading {file.FileName} from Server!", ArkEcho.Resources.LogLevel.Information);
                            AppModel.Instance.AllowLock();
                            return;
                        }

                        exist.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    logInListView($"Exception loading MusicFiles: {ex.Message}", ArkEcho.Resources.LogLevel.Information);
                    AppModel.Instance.AllowLock();
                    return;
                }
            }

            logInListView($"Cleaning Up", ArkEcho.Resources.LogLevel.Information);
            await Task.Delay(200);

            await AppModel.Instance.CleanUpFolder(AppModel.GetAndroidMediaAppSDFolderPath(), exist);

            logInListView($"Success!", ArkEcho.Resources.LogLevel.Information);
            await Task.Delay(1000);

            AppModel.Instance.AllowLock();
        }

        private bool logInListView(string text, Resources.LogLevel level)
        {
            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: {text}");
            adapter.NotifyDataSetChanged();

            logListView.SmoothScrollToPositionFromTop(logListView.LastVisiblePosition, 0);
            return true;
        }
    }
}