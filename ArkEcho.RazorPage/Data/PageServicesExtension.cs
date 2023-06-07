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
            });

            serviceCollection.AddSingleton(environment);

            serviceCollection.AddSingleton<ILocalStorage, StorageImplementation>();
            serviceCollection.AddSingleton<IAppModel, AppModelImplementation>();

            serviceCollection.AddScoped<SnackbarDialogService>();

            serviceCollection.AddScoped<HTMLHelper>();
        }
    }
}
