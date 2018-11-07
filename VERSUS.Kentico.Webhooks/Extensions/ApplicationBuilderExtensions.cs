using Microsoft.AspNetCore.Builder;

using VERSUS.Kentico.Webhooks.Middleware;

namespace VERSUS.Kentico.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebhookMiddleware(this IApplicationBuilder app, string webhookEndpoint)
        {
            return app

                .MapWhen(
                        context =>
                        context.Request.Path.StartsWithSegments(webhookEndpoint) &&
                        context.Request.Headers.Keys.Contains("X-Kc-Signature"),
                        appBranch =>
                        {
                            appBranch.UseMiddleware<WebhookMiddleware>();
                        }
                    );
        }
    }
}