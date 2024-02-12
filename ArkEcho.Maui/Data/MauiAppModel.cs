using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui
{
    public class MauiAppModel : AppModelBase
    {
        public override Player Player { get; protected set; }

        //private SnackbarDialogService snack;
        private IMauiHelper mauiHelper = null;
        private LibrarySync sync;

        public MauiAppModel( /*SnackbarDialogService snack,*/ LibrarySync sync, Logger logger, Rest rest, IMauiHelper mauiHelper) // TODO
            : base(logger, rest)
        {
            //this.snack = snack;
            this.mauiHelper = mauiHelper;
            this.sync = sync;
        }

        protected override async Task<bool> initializePlayer()
        {
            var player = new VLCPlayer(logger);
            Player = player;
            return player.InitPlayer(mauiHelper);
        }

        public override async Task<bool> InitializeOnLogin()
        {
            await base.InitializeOnLogin();

            LibrarySync.LibraryCheckResult result = new LibrarySync.LibraryCheckResult();
            bool success = await sync.CheckLibraryOnStart(Library, result);

            if (!success)
            {
                SetStatus(IAppModel.Status.Connected);
                //snack.CheckingLibraryFailed();
                return false;
            }

            //if (result.FilesMissing)
            //    snack.MusicFilesMissing();

            mauiHelper.SetDragArea(false);
            return success;
        }

        public override Task LogoutUser()
        {
            mauiHelper.SetDragArea(true);
            return base.LogoutUser();
        }
    }
}
