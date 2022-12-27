using ArkEcho.Core;
using Microsoft.Extensions.DependencyInjection;

namespace ArkEcho.RazorPage
{
    public static class MatServiceCollectionExtension
    {
        public static void AddArkEchoServices<T1, T2>(this IServiceCollection serviceCollection, RestLoggingWorker logging, RazorConfig config)
            where T1 : LocalStorageBase
            where T2 : AppModelBase
        {
            serviceCollection.AddSingleton(logging);
            serviceCollection.AddSingleton(config);

            serviceCollection.AddScoped<ILocalStorage, T1>();
            serviceCollection.AddScoped<IAppModel, T2>();
        }
    }
}
