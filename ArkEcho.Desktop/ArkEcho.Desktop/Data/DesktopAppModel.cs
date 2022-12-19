using ArkEcho.Core;
using ArkEcho.RazorPage;
using ArkEcho.VLC;

namespace ArkEcho.Desktop
{
    public class DesktopAppModel : AppModelBase
    {
        public override Player Player { get { return player; } }

        private VLCPlayer player = null;

        public DesktopAppModel(ILocalStorage localStorage, RestLoggingWorker loggingWorker)
            : base(Resources.ARKECHODESKTOP, localStorage, loggingWorker, "https://192.168.178.20:5002", false)
        {
            player = new VLCPlayer();
            syncBase = new DesktopLibrarySync(rest, loggingWorker);
        }

        public override async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return string.Empty;
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            string lib = await rest.GetMusicLibrary();
            await Library.LoadFromJsonString(lib);

            if (!player.InitPlayer())
                return false;

            return true;
        }
    }
}
