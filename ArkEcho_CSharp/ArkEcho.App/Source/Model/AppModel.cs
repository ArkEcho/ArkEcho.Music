using ArkEcho.App.Connection;
using ArkEcho.Core;
using ArkEcho.Player;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArkEcho.App
{
    public class AppModel : IDisposable
    {
        public static AppModel Instance { get; } = new AppModel();

        private AppModel()
        {
            rest = new Connection.ArkEchoRest();
            player = new Player.ArkEchoVLCPlayer();
        }

        public void PlayPause()
        {
            if (player.Playing)
            {
                player.Pause();
            }
            else
            {
                string pathnew = @"/storage/0000-0000/Android/Music/Alligatoah/Triebwerke/Alligatoah - Amnesie.mp3";
                MusicFile file = new MusicFile(pathnew);
                file.LocalFileName = pathnew;

                player.Start(new List<MusicFile> { file }, 0);
            }
        }

        public async Task<bool> Init()
        {
            player.InitPlayer(Log);
            await Task.Delay(5);
            return true;
        }

        public bool Log(string Text, Resources.LogLevel Level)
        {
            // TODO
            return true;
        }

        private ArkEchoRest rest = null;
        private ArkEchoVLCPlayer player = null;
        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {

                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}