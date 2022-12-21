using ArkEcho.Core;
using ArkEcho.RazorPage;
using ArkEcho.VLC;
using System.Diagnostics;

namespace ArkEcho.Desktop
{
    public class DesktopAppModel : AppModelBase
    {
        public override Player Player { get { return player; } }

        public override LibrarySync Sync { get; }

        public override MusicLibrary Library { get; }

        private VLCPlayer player = null;
        private DesktopAppConfig config = null;

        public DesktopAppModel(ILocalStorage localStorage, RestLoggingWorker loggingWorker, DesktopAppConfig config)
            : base(Resources.ARKECHODESKTOP, localStorage, loggingWorker, config.ServerAddress, config.Compression)
        {
            player = new VLCPlayer();
            Sync = new LibrarySync(Resources.ARKECHODESKTOP, rest, loggingWorker);
            Library = new MusicLibrary();

            this.config = config;
        }

        public override async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return await rest.GetAlbumCover(albumGuid);
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            if (!player.InitPlayer())
                return false;

            if (!await LoadLibraryFromServer())
                return false;

            List<MusicFile> missing = new();
            bool success = await Sync.CheckLibrary(config.MusicFolder.LocalPath, Library, new List<MusicFile>(), missing);

            sw.Stop();
            logger.LogDebug($"InitializeLibraryAndPlayer took {sw.ElapsedMilliseconds}ms");

            return !success || missing.Count > 0;
        }

        public override async Task<bool> SynchronizeMusic()
        {
            if (!await LoadLibraryFromServer())
                return false;

            return await Sync.SyncMusicLibrary(config.MusicFolder.LocalPath, Library);
        }
    }
}
