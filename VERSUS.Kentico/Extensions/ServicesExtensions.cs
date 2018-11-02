using System;
using System.Linq;
using System.Reactive.Linq;
using KenticoCloud.Delivery;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using VERSUS.Core;
using VERSUS.Kentico.Areas.WebHooks.Models;
using VERSUS.Kentico.Filters;
using VERSUS.Kentico.Helpers;
using VERSUS.Kentico.Providers;
using VERSUS.Kentico.Services;

namespace VERSUS.Kentico.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddKenticoDelivery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryOptions>(configuration)

                    .AddSingleton<IWebhookListener>(sp => new WebhookListener())
                    .AddSingleton<ICacheManager>(sp => new ReactiveCacheManager(
                        sp.GetRequiredService<IOptions<VersusOptions>>(),
                        sp.GetRequiredService<IMemoryCache>())
                    )
                    .AddScoped<KenticoCloudSignatureActionFilter>()

                    .AddSingleton<IDeliveryClient>(sp => new CachedDeliveryClient(
                        sp.GetRequiredService<ICacheManager>(),
                        new DeliveryClient(sp.GetRequiredService<IOptions<DeliveryOptions>>().Value))
                    {
                        CodeFirstModelProvider = {
                                TypeProvider = new VersusTypeProvider()
                            },
                        ContentLinkUrlResolver = sp.GetRequiredService<IContentLinkUrlResolver>()
                    }
                    );

            var sericeProvider = services.BuildServiceProvider();
            var cacheManager = sericeProvider.GetRequiredService<ICacheManager>();
            var webhookListener = sericeProvider.GetRequiredService<IWebhookListener>();

            Observable.FromEventPattern<CacheInvalidationEventArgs>(webhookListener, nameof(webhookListener.WebhookNotification))
                .Where(e => KenticoCloudCacheHelper.InvalidatingOperations.Any(operation => operation.Equals(e.EventArgs.Operation, StringComparison.Ordinal)))
                .Throttle(TimeSpan.FromSeconds(1))
                .DistinctUntilChanged()
                .Subscribe(e => cacheManager.InvalidateEntry(e.EventArgs.IdentifierSet));

            return services;
        }
    }
}