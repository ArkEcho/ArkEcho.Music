using ArkEcho.Core;
using ArkEcho.RazorPage;
using ArkEcho.VLC;

namespace ArkEcho.Desktop
{
    public class DesktopAppModel : AppModelBase
    {
        public override Player Player { get { return player; } }

        public override LibrarySync Sync { get; }

        private VLCPlayer player = null;
        private DesktopAppConfig config = null;

        public DesktopAppModel(ILocalStorage localStorage, RestLoggingWorker loggingWorker, DesktopAppConfig config)
            : base(Resources.ARKECHODESKTOP, localStorage, loggingWorker, config.ServerAddress, config.Compression)
        {
            player = new VLCPlayer();
            Sync = new LibrarySync(Resources.ARKECHODESKTOP, rest, loggingWorker);
            this.config = config;
        }

        public override async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return string.Empty;
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            string lib = await rest.GetMusicLibrary();
            await Library.LoadFromJsonString(lib);

            // TODO: Aus Datei laden -> wenn nicht vorhanden laden -> bei Sync auch aktualisieren!
            if (!player.InitPlayer())
                return false;

            return true;
        }

        public override async Task SynchronizeMusic()
        {
            await Sync.SyncMusicLibrary(config.MusicFolder.LocalPath, Library);
        }
    }
}
