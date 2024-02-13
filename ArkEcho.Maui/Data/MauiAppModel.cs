using ArkEcho.Core;
using ArkEcho.RazorPage.Data;

namespace ArkEcho.Maui
{
    public class MauiAppModel : AppModelBase
    {
        //private SnackbarDialogService snack;
        private LibrarySync sync;

        public MauiAppModel( /*SnackbarDialogService snack,*/ LibrarySync sync, Logger logger, Rest rest) // TODO
            : base(logger, rest)
        {
            //this.snack = snack;
            this.sync = sync;
        }

        public override async Task<bool> LoadLibraryFromServer()
        {
            await base.LoadLibraryFromServer();

            LibrarySync.LibraryCheckResult result = new LibrarySync.LibraryCheckResult();
            bool success = await sync.CheckLibraryOnStart(Library, result);

            if (!success)
            {
                //snack.CheckingLibraryFailed();
                return false;
            }

            //if (result.FilesMissing)
            //    snack.MusicFilesMissing();

            return success;
        }
    }
}
