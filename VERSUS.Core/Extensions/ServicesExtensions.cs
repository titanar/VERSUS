using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VERSUS.Core.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddCookiePolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(configuration.GetSection("CookiePolicyOptions"))
                    .PostConfigure<CookiePolicyOptions>(options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request
                        options.CheckConsentNeeded = context => true;
                    });

            return services;
        }
    }
}