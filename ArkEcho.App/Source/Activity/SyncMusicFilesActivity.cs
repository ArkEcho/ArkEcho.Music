using Android.App;
using Android.OS;
using Android.Widget;
using ArkEcho.Core;

using System;
using System.IO;

namespace ArkEcho.App
{
    [Activity]
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

            logListView = FindViewById<ListView>(Resource.Id.logListView);
            logListView.Adapter = adapter;

            syncMusicFilesButton = FindViewById<Button>(Resource.Id.syncMusicButton);
            syncMusicFilesButton.Click += onSyncMusicFilesButtonClicked;

            setActionBarButtonMenuHidden(true);
            setActionBarTitleText(GetString(Resource.String.SyncMusicFilesActivityTitle));
        }

        private async void onSyncMusicFilesButtonClicked(object sender, EventArgs e)
        {
            logInListView("Loading Music Library from Remote Server", Core.Resources.LogLevel.Information);

            string libraryString = await AppModel.Instance.Rest.GetMusicLibrary();
            if (string.IsNullOrEmpty(libraryString))
            {
                logInListView("No response from the Server!", Core.Resources.LogLevel.Information);
                return;
            }

            MusicLibrary lib = new MusicLibrary();
            if (!lib.LoadFromJsonString(libraryString))
            {
                logInListView("Cant load json!", Core.Resources.LogLevel.Information);
                return;
            }

            logInListView($"Music File Count: {lib.MusicFiles.Count.ToString()}", Core.Resources.LogLevel.Information);

            MusicFile testFile = lib.MusicFiles.Find(x => x.Title.StartsWith("Vespertilio"));

            if (testFile == null)
            {
                logInListView($"Can't find MusicFile {testFile.FileName}!", Core.Resources.LogLevel.Information);
                return;
            }
            else
                logInListView($"Loading {testFile.FileName}...", Core.Resources.LogLevel.Information);

            byte[] fileBytes = await AppModel.Instance.Rest.GetMusicFile(testFile.GUID);

            if (fileBytes.Length == 0)
            {
                logInListView($"Error loading MusicFile {testFile.FileName} from Server!", Core.Resources.LogLevel.Information);
                return;
            }
            else
                logInListView($"Writing {testFile.FileName}", Core.Resources.LogLevel.Information);

            string sdCardMusicFolder = AppModel.GetAndroidMediaAppSDFolderPath();
            File.Delete($"{sdCardMusicFolder}/{testFile.FileName}");

            using (FileStream stream = new FileStream($"{sdCardMusicFolder}/{testFile.FileName}", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            {
                await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }

            logInListView($"Success!", Core.Resources.LogLevel.Information);
        }

        private bool logInListView(string text, Resources.LogLevel level)
        {
            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: {text}");
            adapter.NotifyDataSetChanged();

            return true;
        }
    }
}