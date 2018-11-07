using Microsoft.Extensions.DependencyInjection;

using VERSUS.Kentico.Webhooks.Middleware;
using VERSUS.Kentico.Webhooks.Services;

namespace VERSUS.Kentico.Webhooks.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddKenticoWebhooks(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IWebhookListener<>), typeof(WebhookListener<>))
                    .AddTransient<WebhookMiddleware>()
                    ;

            return services;
        }
    }
}