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

            //string sdCardMusicFolder = ArkEcho.App.AppModel.GetAndroidMediaAppSDFolderPath();

            //try
            //{
            //    using (FileStream stream = new FileStream($"{sdCardMusicFolder}/tester.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            //    {
            //        byte[] test = Encoding.UTF8.GetBytes("Test");
            //        stream.Write(test, 0, test.Length);
            //    }
            //    using (FileStream stream2 = new FileStream($"{sdCardMusicFolder}/tester.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
            //    {
            //        byte[] test2 = Encoding.UTF8.GetBytes("Hello World");
            //        stream2.Write(test2, 0, test2.Length);
            //    }
            //    File.Delete($"{sdCardMusicFolder}/tester.txt");
            //}
            //catch (Exception ex)
            //{
            //    logInListView(ex.Message, Core.Resources.LogLevel.Error);
            //}

            string libraryString = await arkEchoRest.GetMusicLibrary();
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

            logInListView(lib.MusicFiles.Count.ToString(), Core.Resources.LogLevel.Information);
        }

        private bool logInListView(string text, Resources.LogLevel level)
        {
            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: {text}");
            adapter.NotifyDataSetChanged();

            return true;
        }
    }
}