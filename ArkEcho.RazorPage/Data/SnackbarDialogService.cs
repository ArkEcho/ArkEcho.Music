using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ArkEcho.RazorPage.Data
{
    public class SnackbarDialogService
    {
        private ISnackbar snackbarProvider;
        private NavigationManager navigation;
        private Snackbar musicFilesMissingSnackbar;

        public SnackbarDialogService()
        {
        }

        public void Initialize(ISnackbar snackbar, NavigationManager navigation)
        {
            this.snackbarProvider = snackbar;
            this.navigation = navigation;
        }

        public void CloseOnLogout()
        {
            if (musicFilesMissingSnackbar != null)
            {
                snackbarProvider.Remove(musicFilesMissingSnackbar);
                musicFilesMissingSnackbar = null;
            }
        }

        public void WrongUserCredentials()
        {
            snackbarProvider.Add($"Username or Password wrong!", Severity.Error, config => { config.ShowCloseIcon = true; });
        }

        public void MusicFolderChanged()
        {
            snackbarProvider.Add("Music Folder changed successfully!", Severity.Success, config => { config.ShowCloseIcon = true; });
        }

        public void CheckingLibraryFailed()
        {
            snackbarProvider.Add("Checking Library Failed!", Severity.Info, config =>
            {
                config.SnackbarVariant = Variant.Outlined;
            });
        }

        public void MusicFilesMissing()
        {
            if (musicFilesMissingSnackbar != null)
                return;

            musicFilesMissingSnackbar = snackbarProvider.Add("Some Files are missing", Severity.Info, config =>
            {
                config.ShowCloseIcon = true;
                config.SnackbarVariant = Variant.Outlined;
                config.RequireInteraction = true;
                config.HideTransitionDuration = 250;

                config.Action = "Sync";
                config.ActionColor = Color.Tertiary;
                config.ActionVariant = Variant.Filled;
                config.Onclick = snackbar =>
                {
                    navigation.NavigateTo("/Sync");
                    return Task.CompletedTask;
                };
            });
        }
    }
}
