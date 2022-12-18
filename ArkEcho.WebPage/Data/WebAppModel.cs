using ArkEcho.Core;
using ArkEcho.RazorPage;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override MusicLibrary Library { get; } = null;

        public override Player Player { get { return jsPlayer; } }

        private bool initialized = false;
        private JSPlayer jsPlayer = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage) : base(localStorage, WebPageManager.Instance.Config.ServerAddress, WebPageManager.Instance.Config.Compression)
        {
            Library = new MusicLibrary();
            jsPlayer = new JSPlayer(jsRuntime, WebPageManager.Instance.Config.ServerAddress);
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            if (initialized)
                return true;

            string lib = await rest.GetMusicLibrary();
            await Library.LoadFromJsonString(lib);

            if (Library.MusicFiles.Count > 0)
            {
                Console.WriteLine($"AppModel initialized, {Library.MusicFiles.Count}");

                if (jsPlayer.InitPlayer())
                {
                    initialized = true;
                    return true;
                }
            }

            Console.WriteLine($"Error initializing AppModel");
            return false;
        }

        public override async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return await rest.GetAlbumCover(albumGuid);
        }
    }
}
