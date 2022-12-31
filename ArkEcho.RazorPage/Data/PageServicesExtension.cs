using ArkEcho.Core;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace ArkEcho.RazorPage
{
    public static class ServiceCollectionExtension
    {
        public static void AddArkEchoServices<T1, T2>(this IServiceCollection serviceCollection, RestLoggingWorker logging, RazorConfig config)
            where T1 : LocalStorageBase
            where T2 : AppModelBase
        {
            serviceCollection.AddMudServices();

            serviceCollection.AddSingleton(logging);
            serviceCollection.AddSingleton(config);

            serviceCollection.AddScoped<ILocalStorage, T1>();
            serviceCollection.AddScoped<IAppModel, T2>();
        }
    }
}
