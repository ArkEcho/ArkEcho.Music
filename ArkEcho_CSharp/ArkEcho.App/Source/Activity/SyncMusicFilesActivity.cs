using Android.App;
using Android.OS;
using Android.Widget;
using ArkEcho.Core;
using System;
using System.Collections.Generic;

namespace ArkEcho.App
{
    [Activity]
    public class SyncMusicFilesActivity : ExtendedActivity
    {
        Button syncMusicFilesButton = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SyncMusicFiles);

            syncMusicFilesButton = FindViewById<Button>(Resource.Id.syncMusicButton);
            syncMusicFilesButton.Click += onSyncMusicFilesButtonClicked;
        }

        private void onSyncMusicFilesButtonClicked(object sender, EventArgs e)
        {
            string sdCardMusicFolder = ArkEcho.App.AppModel.GetMusicSDFolderPath();

            string pathnew = $"{sdCardMusicFolder}Alligatoah/Triebwerke/Alligatoah - Amnesie.mp3";
            MusicFile file = new MusicFile(pathnew);
            file.LocalFileName = pathnew;

            AppModel.Instance.Player.Start(new List<MusicFile> { file }, 0);
        }
    }
}