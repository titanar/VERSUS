using Microsoft.AspNetCore.Builder;

using VERSUS.Infrastructure.Services;

namespace VERSUS.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSignalR(this IApplicationBuilder app)
        {
            return app

                .UseSignalR(routes =>
                {
                    routes.MapHub<SiteHub>("/siteHub");
                });
        }
    }
}