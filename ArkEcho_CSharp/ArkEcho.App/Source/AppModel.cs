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

namespace ArkEcho.App.Source
{
    public class AppModel : IDisposable
    {
        public static AppModel Instance { get; } = new AppModel();

        private AppModel()
        {
            rest = new Connection.ArkEchoRest();
        }

        public async Task<bool> Init()
        {
            // Prepare WebSockets Connection
            Websockets.Droid.WebsocketConnection.Link();

            await rest.getMusic();

            return true;
        }

        private ArkEchoRest rest;

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