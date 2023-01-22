using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui
{
    public class MauiAppModel : AppModelBase
    {
        public override Player Player { get { return player; } }

        public override LibrarySync Sync { get; }

        public override string MusicFolder { get { return getMusicSyncPath(); } }

        private VLCPlayer player = null;
        private IMauiHelper mauiHelper = null;

        public MauiAppModel(ILocalStorage localStorage, AppEnvironment environment, IMauiHelper mauiHelper)
            : base(environment, localStorage)
        {
            player = new VLCPlayer(logger);
            this.mauiHelper = mauiHelper;
            Sync = new LibrarySync(environment, rest, new RestLogger(environment, "LibrarySync", rest));
        }

        public override async Task<bool> InitializeOnLoad()
        {
            if (!await rest.CheckConnection())
                return false;

            logger.LogStatic($"Executing on {environment.Platform}");

            if (!player.InitPlayer())
                return false;

            return await LoadLibraryFromServer();
        }

        public override async Task<bool> InitializeOnLogin()
        {
            List<MusicFile> missing = new();
            bool success = await Sync.CheckLibrary(getMusicSyncPath(), Library, new List<MusicFile>(), missing);

            return !success || missing.Count > 0;
        }

        public override async Task<bool> StartSynchronizeMusic()
        {
            if (!await LoadLibraryFromServer())
                return false;

            return await Sync.StartSyncMusicLibrary(getMusicSyncPath(), Library);
        }

        private string getMusicSyncPath()
        {
            return mauiHelper.GetPlatformSpecificMusicFolder(GetLoggedInUser());
        }
    }
}
