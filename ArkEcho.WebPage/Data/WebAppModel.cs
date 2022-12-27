using ArkEcho.Core;
using ArkEcho.RazorPage;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override Player Player { get { return jsPlayer; } }
        public override LibrarySync Sync { get; }

        private bool initialized = false;
        private JSPlayer jsPlayer = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage, RestLoggingWorker loggingWorker, RazorConfig config)
            : base(Resources.ARKECHOWEBPAGE, localStorage, loggingWorker, config)
        {
            jsPlayer = new JSPlayer(jsRuntime, logger, config.ServerAddress);
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            if (initialized)
                return true;

            string lib = await rest.GetMusicLibrary();
            await Library.LoadFromJsonString(lib);

            if (Library.MusicFiles.Count > 0)
            {
                logger.LogStatic($"AppModel initialized, {Library.MusicFiles.Count}");

                if (jsPlayer.InitPlayer()) // TODO entfernen
                {
                    initialized = true;
                    return true;
                }
            }

            logger.LogStatic($"Error initializing AppModel");
            return false;
        }

        public override async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return await rest.GetAlbumCover(albumGuid);
        }

        public override Task SynchronizeMusic()
        {
            throw new NotImplementedException();
        }
    }
}
