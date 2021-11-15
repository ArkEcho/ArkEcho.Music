using Android.App;
using Android.OS;
using Android.Widget;
using ArkEcho.App.Connection;
using ArkEcho.Core;

using System;

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
            logInListView("Loading Music Library from Remote Server", Core.Resources.LogLevel.Information);

            string sdCardMusicFolder = ArkEcho.App.AppModel.GetMusicSDFolderPath();

            //if ((ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            //|| (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted))
            //{
            //    ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, REQUEST);
            //}

            //string[] PERMISSIONS_TO_REQUEST = { Manifest.Permission.WriteExternalStorage };
            //RequestPermissions(PERMISSIONS_TO_REQUEST, 1000);


            //File.Create($"{AppModel.GetMusicSDFolderPath()}/test2.txt");

            //string test = File.ReadAllText($"{AppModel.GetMusicSDFolderPath()}/test.txt");
            //File.WriteAllText($"{AppModel.GetMusicSDFolderPath()}/test.txt", "Test");


            string libraryString = await arkEchoRest.GetMusicLibrary();


            MusicLibrary lib = new MusicLibrary();
            if (!string.IsNullOrEmpty(libraryString))
            {
                if (lib.LoadFromJsonString(libraryString))
                    logInListView(lib.MusicFiles.Count.ToString(), Core.Resources.LogLevel.Information);
            }
        }

        private bool logInListView(string text, Resources.LogLevel level)
        {
            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: {text}");
            adapter.NotifyDataSetChanged();
            return true;
        }
    }
}