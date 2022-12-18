using ArkEcho.Core;
using ArkEcho.RazorPage;

namespace ArkEcho.Desktop.Data
{
    public class DesktopAppModel : AppModelBase
    {
        public override MusicLibrary Library => throw new NotImplementedException();

        public override Player Player => throw new NotImplementedException();

        public DesktopAppModel(ILocalStorage localStorage) : base(localStorage, "https://192.168.178.20:5002", false)
        {

        }

        public override async Task<string> GetAlbumCover(Guid albumGuid)
        {
            return string.Empty;
        }

        public override async Task<bool> InitializeLibraryAndPlayer()
        {
            return true;
        }
    }
}
