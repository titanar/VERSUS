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
                    .AddSingleton<IContentLinkUrlResolver, ContentLinkUrlResolver>()

                    .AddSingleton<IDeliveryClient>(sp =>
                    new CachedDeliveryClient(sp.GetRequiredService<ICacheManager>(),
                        new DeliveryClient(sp.GetRequiredService<IOptionsSnapshot<DeliveryOptions>>().Value))
                    {
                        CodeFirstModelProvider = { TypeProvider = new ContentTypeProvider() },
                        ContentLinkUrlResolver = sp.GetRequiredService<IContentLinkUrlResolver>()
                    });

            var sericeProvider = services.BuildServiceProvider();
            var versusOptions = sericeProvider.GetRequiredService<IOptionsSnapshot<VersusOptions>>().Value;

            HtmlHelperExtensions.ResponsiveImagesEnabled = versusOptions.ResponsiveImagesEnabled;
            HtmlHelperExtensions.ResponsiveWidths = versusOptions.ResponsiveWidths;

            return services;
        }
    }
}