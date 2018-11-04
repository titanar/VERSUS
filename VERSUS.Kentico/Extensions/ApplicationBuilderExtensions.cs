using Microsoft.AspNetCore.Builder;
using VERSUS.Kentico.Middleware;

namespace VERSUS.Kentico.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseWebhookMiddleware(this IApplicationBuilder app)
        {
            return app

                .MapWhen(
                        context =>
                        context.Request.Path.StartsWithSegments("/Webhooks/KenticoCloud") &&
                        context.Request.Headers.Keys.Contains("X-Kc-Signature"),
                        appBranch =>
                        {
                            appBranch.UseMiddleware<WebhookMiddleware>();
                        }
                    );
        }
    }
}