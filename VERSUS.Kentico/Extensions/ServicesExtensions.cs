using KenticoCloud.Delivery;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using VERSUS.Core;
using VERSUS.Kentico.Middleware;
using VERSUS.Kentico.Providers;
using VERSUS.Kentico.Services;
using VERSUS.Kentico.Webhooks.Services;

namespace VERSUS.Kentico.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddKenticoDelivery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryOptions>(configuration)

                    .AddSingleton<ICacheManager>(sp => new CacheManager(
                        sp.GetRequiredService<IOptionsSnapshot<VersusOptions>>(),
                        sp.GetRequiredService<IMemoryCache>())
                    )
                    .AddSingleton<IWebhookListener>(sp => new WebhookListener(sp.GetRequiredService<ICacheManager>()))
                    .AddTransient<WebhookMiddleware>()

                    .AddSingleton<IDeliveryClient>(sp => new CachedDeliveryClient(
                        sp.GetRequiredService<ICacheManager>(),
                        new DeliveryClient(sp.GetRequiredService<IOptionsSnapshot<DeliveryOptions>>().Value))
                    {
                        CodeFirstModelProvider = {
                                TypeProvider = new VersusTypeProvider()
                            },
                        ContentLinkUrlResolver = sp.GetRequiredService<IContentLinkUrlResolver>()
                    }
                    )
                    ;

            var sericeProvider = services.BuildServiceProvider();
            var versusOptions = sericeProvider.GetRequiredService<IOptionsSnapshot<VersusOptions>>().Value;

            HtmlHelperExtensions.ResponsiveImagesEnabled = versusOptions.ResponsiveImagesEnabled;
            HtmlHelperExtensions.ResponsiveWidths = versusOptions.ResponsiveWidths;

            return services;
        }
    }
}