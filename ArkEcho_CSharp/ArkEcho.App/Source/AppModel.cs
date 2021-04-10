using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ArkEcho.App.Connection;
using ArkEcho.Player;

namespace ArkEcho.App
{
    public class AppModel : IDisposable
    {
        public static AppModel Instance { get; } = new AppModel();

        private AppModel()
        {
            rest = new Connection.ArkEchoRest();
            player = new Player.ArkEchoPlayer();
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

                player.Play(pathnew);
            }
        }

        public async Task<bool> Init()
        {
            // Prepare WebSockets Connection
            Websockets.Droid.WebsocketConnection.Link();

            //await rest.getMusicFileInfo();

            return true;
        }

        private ArkEchoRest rest = null;
        private ArkEchoPlayer player = null;
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