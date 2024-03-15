using ArkEcho.Core;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;

namespace ArkEcho.RazorPage.Data
{
    public static class ServiceCollectionExtension
    {
        public static void AddArkEchoServices<StorageImplementation, LibraryControllerImplementation, PlayerImplementation, PlatformControllerImplementation>
            (this IServiceCollection serviceCollection, AppEnvironment environment)
            where StorageImplementation : LocalStorageBase
            where LibraryControllerImplementation : LibraryControllerBase
            where PlayerImplementation : Player
            where PlatformControllerImplementation : PlatformControllerBase
        {
            serviceCollection.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
            });

            serviceCollection.AddSingleton(environment);

            serviceCollection.AddSingleton<Logger, RestLogger>();
            serviceCollection.AddSingleton<Rest>();
            serviceCollection.AddSingleton<Authentication>();
            serviceCollection.AddSingleton<ConnectionStatus>();

            serviceCollection.AddSingleton<ILocalStorage, StorageImplementation>();
            serviceCollection.AddSingleton<Player, PlayerImplementation>();
            serviceCollection.AddSingleton<LibraryControllerBase, LibraryControllerImplementation>();
            serviceCollection.AddSingleton<PlatformControllerBase, PlatformControllerImplementation>();

            serviceCollection.AddSingleton<SnackbarDialogService>();

            serviceCollection.AddSingleton<ScrollPositionService>();
            serviceCollection.AddSingleton<HTMLHelper>();
        }
    }
}
