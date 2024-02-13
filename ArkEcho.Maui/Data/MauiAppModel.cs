using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui
{
    public class MauiAppModel : AppModelBase
    {
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

        public override async Task<bool> InitializeOnLogin()
        {
            await base.InitializeOnLogin();

            LibrarySync.LibraryCheckResult result = new LibrarySync.LibraryCheckResult();
            bool success = await sync.CheckLibraryOnStart(Library, result);

            if (!success)
            {
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
