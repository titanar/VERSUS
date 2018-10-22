using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

using KenticoCloud.Delivery;

namespace VERSUS.Kentico
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddKenticoDelivery(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DeliveryOptions>(configuration);

            // Singleton instances of Delivery client and resolver classes
            services.AddSingleton<IDeliveryClient, DeliveryClient>();
            services.AddSingleton<IContentLinkUrlResolver, VersusContentLinkUrlResolver>();
            services.AddSingleton<ICodeFirstTypeProvider, VersusTypeProvider>();

            return services;
        }
    }
}
