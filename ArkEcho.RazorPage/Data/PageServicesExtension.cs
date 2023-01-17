using ArkEcho.Core;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace ArkEcho.RazorPage.Data
{
    public static class ServiceCollectionExtension
    {
        public static void AddArkEchoServices<T1, T2>(this IServiceCollection serviceCollection, AppEnvironment environment)
            where T1 : LocalStorageBase
            where T2 : AppModelBase
        {
            serviceCollection.AddMudServices();

            serviceCollection.AddSingleton(environment);

            serviceCollection.AddSingleton<ILocalStorage, T1>();
            serviceCollection.AddSingleton<IAppModel, T2>();

            serviceCollection.AddScoped<HTMLHelper>();
        }
    }
}
