﻿using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using ArkEcho.Core;

using System;
using System.Collections.Generic;
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

            MusicLibrary lib = null;
            try
            {
                logInListView("Loading MusicLibrary", Core.Resources.LogLevel.Information);

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

                lib = AppModel.Instance.Library;
                logInListView($"Music File Count: {lib.MusicFiles.Count.ToString()}", Core.Resources.LogLevel.Information);

                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                logInListView($"Exception loading MusicLibrary: {ex.Message}", Core.Resources.LogLevel.Information);
                AppModel.Instance.AllowLock();
                return;
            }

            logInListView($"Loading and Checking Files", Core.Resources.LogLevel.Information);
            await Task.Delay(200);

            List<string> okFiles = new List<string>();
            try
            {
                foreach (MusicFile file in lib.MusicFiles)
                {
                    file.Folder = getMusicFileFolder(file, lib);
                    if (string.IsNullOrEmpty(file.Folder))
                    {
                        logInListView($"Error building Path for {file.FileName}", Core.Resources.LogLevel.Information);
                        AppModel.Instance.AllowLock();
                        return;
                    }

                    okFiles.Add(file.GetFullFilePath());

                    if (!Directory.Exists(file.Folder))
                        Directory.CreateDirectory(file.Folder);

                    if (File.Exists(file.GetFullFilePath()))
                        continue;

                    logInListView($"Loading {file.FileName}", Core.Resources.LogLevel.Information);

                    byte[] fileBytes = await AppModel.Instance.Rest.GetMusicFile(file.GUID);

                    if (fileBytes.Length == 0)
                    {
                        logInListView($"Error loading {file.FileName} from Server!", Core.Resources.LogLevel.Information);
                        AppModel.Instance.AllowLock();
                        return;
                    }

                    using (FileStream stream = new FileStream(file.GetFullFilePath(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                    {
                        await stream.WriteAsync(fileBytes, 0, fileBytes.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                logInListView($"Exception loading MusicFiles: {ex.Message}", Core.Resources.LogLevel.Information);
                AppModel.Instance.AllowLock();
                return;
            }

            logInListView($"Cleaning Up", Core.Resources.LogLevel.Information);
            await Task.Delay(200);

            cleanUpFolder(AppModel.GetAndroidMediaAppSDFolderPath(), okFiles);

            logInListView($"Success!", Core.Resources.LogLevel.Information);
            await Task.Delay(1000);

            AppModel.Instance.AllowLock();
        }

        private void cleanUpFolder(string folder, List<string> okFiles)
        {
            foreach (string subFolder in Directory.GetDirectories(folder))
                cleanUpFolder(subFolder, okFiles);

            foreach (string file in Directory.GetFiles(folder))
            {
                if (!string.IsNullOrEmpty(okFiles.Find(x => x.Equals(file, StringComparison.OrdinalIgnoreCase))))
                    continue;
                else
                    File.Delete(file);
            }

            if (Directory.GetDirectories(folder).Length == 0 && Directory.GetFiles(folder).Length == 0)
                Directory.Delete(folder);
        }

        private string getMusicFileFolder(MusicFile file, MusicLibrary lib)
        {
            string mediaFolderPath = AppModel.GetAndroidMediaAppSDFolderPath();
            Album album = lib.Album.Find(x => x.GUID == file.Album);
            AlbumArtist artist = lib.AlbumArtists.Find(x => x.GUID == file.AlbumArtist);

            if (album == null || artist == null)
                return string.Empty;

            return $"{mediaFolderPath}/{artist.Name}/{album.Name}";
        }

        private void logInListView(string text, Resources.LogLevel level)
        {
            adapter.Add($"{DateTime.Now:HH:mm:ss:fff}: {text}");
            adapter.NotifyDataSetChanged();

            logListView.SmoothScrollToPositionFromTop(logListView.LastVisiblePosition, 0);
        }
    }
}