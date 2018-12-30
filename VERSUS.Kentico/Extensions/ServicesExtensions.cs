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
            services.Configure<KenticoOptions>(configuration.GetSection("KenticoOptions"))

                    .AddSingleton<ICacheManager, CacheManager>()
                    .AddTransient<IDependencyResolver, DependencyResolver>()
                    .AddTransient<ICodeFirstTypeProvider, ContentTypeProvider>()
                    .AddTransient<IContentLinkUrlResolver, ContentLinkUrlResolver>()

                    .AddDeliveryClient(configuration)

                    .AddScoped<IDeliveryClient>(sp => new CachedDeliveryClient(
                         sp.GetRequiredService<ICacheManager>(),
                         DeliveryClientBuilder
                             .WithOptions(_ => sp.GetRequiredService<IOptionsSnapshot<KenticoOptions>>().Value)
                             .WithCodeFirstTypeProvider(sp.GetRequiredService<ICodeFirstTypeProvider>())
                             .WithContentLinkUrlResolver(sp.GetRequiredService<IContentLinkUrlResolver>())
                             .Build(),
                         CreatePreviewDeliveryClientOrNull(sp),
                         sp.GetRequiredService<IDependencyResolver>()
                     ));

            var kenticoOptions = services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<KenticoOptions>>().Value;

            HtmlHelperExtensions.ResponsiveImagesEnabled = kenticoOptions.ResponsiveImagesEnabled;
            HtmlHelperExtensions.ResponsiveWidths = kenticoOptions.ResponsiveWidths;

            return services;
        }

        private static IDeliveryClient CreatePreviewDeliveryClientOrNull(IServiceProvider sp)
        {
            var kenticoOptions = sp.GetRequiredService<IOptionsSnapshot<KenticoOptions>>().Value;

            if (!string.IsNullOrEmpty(kenticoOptions.PreviewApiKey))
            {
                kenticoOptions.UsePreviewApi = true;
                kenticoOptions.UseSecuredProductionApi = false;

                return DeliveryClientBuilder
                    .WithOptions(_ => kenticoOptions)
                    .WithCodeFirstTypeProvider(sp.GetRequiredService<ICodeFirstTypeProvider>())
                    .WithContentLinkUrlResolver(sp.GetRequiredService<IContentLinkUrlResolver>())
                    .Build();
            }

            return null;
        }
    }
}