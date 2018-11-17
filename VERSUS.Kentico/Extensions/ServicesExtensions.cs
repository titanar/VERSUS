using System;
using KenticoCloud.Delivery;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using VERSUS.Core;
using VERSUS.Kentico.Providers;
using VERSUS.Kentico.Services;

namespace VERSUS.Kentico.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddKenticoDelivery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryOptions>(configuration)

                    .AddSingleton<ICacheManager, CacheManager>()
                    .AddTransient<ICodeFirstTypeProvider, ContentTypeProvider>()
                    .AddTransient<IContentLinkUrlResolver, ContentLinkUrlResolver>()

                    .AddDeliveryClient(configuration)

                    .AddSingleton<IDeliveryClient>(sp => new CachedDeliveryClient(
                          sp.GetRequiredService<ICacheManager>(),
                         DeliveryClientBuilder
                             .WithOptions(_ => sp.GetRequiredService<IOptionsSnapshot<DeliveryOptions>>().Value)
                             .WithCodeFirstTypeProvider(sp.GetRequiredService<ICodeFirstTypeProvider>())
                             .WithContentLinkUrlResolver(sp.GetRequiredService<IContentLinkUrlResolver>())
                             .Build(),
                         CreatePreviewDeliveryClientOrNull(sp)
                     ));

            var sericeProvider = services.BuildServiceProvider();
            var versusOptions = sericeProvider.GetRequiredService<IOptionsSnapshot<VersusOptions>>().Value;

            HtmlHelperExtensions.ResponsiveImagesEnabled = versusOptions.ResponsiveImagesEnabled;
            HtmlHelperExtensions.ResponsiveWidths = versusOptions.ResponsiveWidths;

            return services;
        }

        private static IDeliveryClient CreatePreviewDeliveryClientOrNull(IServiceProvider sp)
        {
            var deliveryOptions = sp.GetRequiredService<IOptionsSnapshot<DeliveryOptions>>().Value;

            if (!string.IsNullOrEmpty(deliveryOptions.PreviewApiKey))
            {
                deliveryOptions.UsePreviewApi = true;
                deliveryOptions.UseSecuredProductionApi = false;

                return DeliveryClientBuilder
                    .WithOptions(_ => deliveryOptions)
                    .WithCodeFirstTypeProvider(sp.GetRequiredService<ICodeFirstTypeProvider>())
                    .WithContentLinkUrlResolver(sp.GetRequiredService<IContentLinkUrlResolver>())
                    .Build();
            }

            return null;
        }
    }
}