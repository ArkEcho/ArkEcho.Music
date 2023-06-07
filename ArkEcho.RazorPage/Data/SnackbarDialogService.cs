using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace ArkEcho.RazorPage.Data
{
    public class SnackbarDialogService
    {
        private ISnackbar snackbar;
        private NavigationManager navigation;

        public SnackbarDialogService(ISnackbar snackbar, NavigationManager navigation)
        {
            this.snackbar = snackbar;
            this.navigation = navigation;
        }

        public void CheckingLibraryFailed()
        {
            snackbar.Add("Checking Library Failed!", Severity.Info, config =>
            {
                config.SnackbarVariant = Variant.Outlined;
            });
        }

        public void MusicFilesMissing()
        {
            snackbar.Add("Some Files are missing", Severity.Info, config =>
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
