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
    public class ArkEchoApp : IDisposable
    {
        public static ArkEchoApp Instance { get; } = new ArkEchoApp();

        public ArkEchoApp()
        {

        }

        public async Task<bool> Init()
        {
            rest = new Connection.ArkEchoRest();
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