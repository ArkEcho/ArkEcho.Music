using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using ArkEcho.Core;

using System;
using System.IO;
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
            logInListView("Loading Music Library from Remote Server", Core.Resources.LogLevel.Information);

            string libraryString = await AppModel.Instance.Rest.GetMusicLibrary();
            if (string.IsNullOrEmpty(libraryString))
            {
                logInListView("No response from the Server!", Core.Resources.LogLevel.Information);
                AppModel.Instance.AllowLock();
                return;
            }

            if (!AppModel.Instance.SetMusicLibrary(libraryString))
            {
                logInListView("Cant load json!", Core.Resources.LogLevel.Information);
                AppModel.Instance.AllowLock();
                return;
            }

            MusicLibrary lib = AppModel.Instance.Library;
            logInListView($"Music File Count: {lib.MusicFiles.Count.ToString()}", Core.Resources.LogLevel.Information);

            string mediaFolderPath = AppModel.GetAndroidMediaAppSDFolderPath();
            foreach (MusicFile file in lib.MusicFiles.FindAll(x => x.AlbumArtist == lib.AlbumArtists.Find(y => y.Name.Equals("Alligatoah")).GUID))
            {
                logInListView($"Loading {file.FileName}...", Core.Resources.LogLevel.Information);

                file.Folder = $"{mediaFolderPath}/{lib.AlbumArtists.Find(x => x.GUID == file.AlbumArtist).Name}/{lib.Album.Find(x => x.GUID == file.Album).Name}";

                if (!Directory.Exists(file.Folder))
                    Directory.CreateDirectory(file.Folder);

                if (File.Exists(file.GetFullFilePath()))
                    continue;

                byte[] fileBytes = await AppModel.Instance.Rest.GetMusicFile(file.GUID);

                if (fileBytes.Length == 0)
                {
                    logInListView($"Error loading MusicFile {file.FileName} from Server!", Core.Resources.LogLevel.Information);
                    return;
                }
                else
                    logInListView($"Writing {file.FileName}", Core.Resources.LogLevel.Information);

                using (FileStream stream = new FileStream(file.GetFullFilePath(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }
            }

            logInListView($"Success!", Core.Resources.LogLevel.Information);

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