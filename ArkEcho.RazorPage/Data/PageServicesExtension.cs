using ArkEcho.Core;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace ArkEcho.RazorPage.Data
{
    public static class ServiceCollectionExtension
    {
        public static void AddArkEchoServices<StorageImplementation, AppModelImplementation>(this IServiceCollection serviceCollection, AppEnvironment environment)
            where StorageImplementation : LocalStorageBase
            where AppModelImplementation : AppModelBase
        {
            serviceCollection.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 10000;
                config.SnackbarConfiguration.HideTransitionDuration = 500;
                config.SnackbarConfiguration.ShowTransitionDuration = 500;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
            });

            serviceCollection.AddSingleton(environment);

            serviceCollection.AddSingleton<ILocalStorage, StorageImplementation>();
            serviceCollection.AddSingleton<IAppModel, AppModelImplementation>();

            serviceCollection.AddScoped<HTMLHelper>();
        }
    }
}
