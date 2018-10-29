using KenticoCloud.Delivery;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using VERSUS.Core;
using VERSUS.Kentico.Filters;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Providers;
using VERSUS.Kentico.Resolvers;
using VERSUS.Kentico.Services;

namespace VERSUS.Kentico.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddKenticoDelivery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryOptions>(configuration)

                    .AddSingleton<IWebhookListener>(sp => new WebhookListener())
                    .AddSingleton<IDependentTypesResolver>(sp => new DependentTypesResolver())
                    .AddSingleton<ICacheManager>(sp => new ReactiveCacheManager(
                        sp.GetRequiredService<IOptions<VersusOptions>>(),
                        sp.GetRequiredService<IMemoryCache>(),
                        sp.GetRequiredService<IDependentTypesResolver>(),
                        sp.GetRequiredService<IWebhookListener>())
                    )
                    .AddScoped<KenticoCloudSignatureActionFilter>()

                    .AddSingleton<IDeliveryClient>(sp => new CachedDeliveryClient(
                        sp.GetRequiredService<IOptions<VersusOptions>>(),
                        sp.GetRequiredService<ICacheManager>(),
                        new DeliveryClient(sp.GetRequiredService<IOptions<DeliveryOptions>>().Value))
                        {
                            CodeFirstModelProvider = {
                                TypeProvider = new VersusTypeProvider()
                            },
                            ContentLinkUrlResolver = sp.GetRequiredService<IContentLinkUrlResolver>()
                        }
                    );

            return services;
        }
    }
}
