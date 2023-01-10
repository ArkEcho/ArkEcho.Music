using ArkEcho.Core;
using ArkEcho.RazorPage.Data;
using System.Diagnostics;

namespace ArkEcho.Maui
{
    public class MauiAppModel : AppModelBase
    {
        public override Player Player { get { return player; } }

        public override LibrarySync Sync { get; }

        private VLCPlayer player = null;

        public MauiAppModel(ILocalStorage localStorage, RestLoggingWorker loggingWorker, RazorConfig config)
            : base(Resources.ARKECHOMAUI, localStorage, loggingWorker, config)
        {
            player = new VLCPlayer(logger);
            Sync = new LibrarySync(Resources.ARKECHOMAUI, rest, loggingWorker);
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
            bool success = await Sync.CheckLibrary(Config.MusicFolder.LocalPath, Library, new List<MusicFile>(), missing);

            sw.Stop();
            logger.LogDebug($"InitializeLibraryAndPlayer took {sw.ElapsedMilliseconds}ms");

            return !success || missing.Count > 0;
        }

        public override async Task<bool> StartSynchronizeMusic()
        {
            if (!await LoadLibraryFromServer())
                return false;

            return await Sync.StartSyncMusicLibrary(Config.MusicFolder.LocalPath, Library);
        }
    }
}
