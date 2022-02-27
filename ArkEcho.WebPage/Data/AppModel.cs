using ArkEcho.Core;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class AppModel
    {
        // TODO: private?
        public MusicLibrary Library { get; private set; } = null;

        public ArkEchoRest Rest { get; private set; } = null;

        public ArkEchoJSPlayer Player { get; set; } = null;

        private bool initialized = false;

        public AppModel()
        {
            Rest = new ArkEchoRest("https://192.168.178.20:5002", false);
        }

        public async Task<bool> Initialize(IJSRuntime jsRuntime)
        {
            if (initialized)
                return true;

            if (Library == null)
            {
                Library = new MusicLibrary();
                string lib = await Rest.GetMusicLibrary();
                await Library.LoadFromJsonString(lib);

                if (Library.MusicFiles.Count > 0)
                {
                    Console.WriteLine($"AppModel initialized, {Library.MusicFiles.Count}");

                    Player = new ArkEchoJSPlayer();
                    if (Player.InitPlayer(jsRuntime))
                    {
                        initialized = true;
                        return true;
                    }
                }
            }

            Console.WriteLine($"Error initializing AppModel");
            return false;
        }
    }
}
