using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using Microsoft.JSInterop;

namespace ArkEcho.WebPage
{
    public class WebAppModel : AppModelBase
    {
        public override Player Player { get { return jsPlayer; } }
        public override LibrarySync Sync { get; }

        private bool initialized = false;
        private JSPlayer jsPlayer = null;

        public WebAppModel(IJSRuntime jsRuntime, ILocalStorage localStorage, Rest rest, RestLoggingWorker loggingWorker, RazorConfig config)
            : base(Resources.ARKECHOWEBPAGE, localStorage, rest, loggingWorker, config)
        {
            jsPlayer = new JSPlayer(jsRuntime, logger, config.ServerAddress);
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            if (initialized)
                return true;

            if (!jsPlayer.InitPlayer())
                return false;

            if (!await LoadLibraryFromServer())
                return false;

            if (Library.MusicFiles.Count > 0)
            {
                logger.LogStatic($"AppModel initialized, {Library.MusicFiles.Count}");
                initialized = true;
                return true;
            }
            else
            {
                logger.LogStatic($"Error initializing AppModel");
                return false;
            }
        }

        public override Task StartSynchronizeMusic()
        {
            throw new NotImplementedException();
        }
    }
}
