using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using VERSUS.Core;
using VERSUS.Infrastructure.Models;
using VERSUS.Infrastructure.Services;

namespace VERSUS.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment env)
        {
            var versusOptions = services.BuildServiceProvider().GetRequiredService<IOptionsSnapshot<VersusOptions>>().Value;

            ObservableExtensions.DefaultTimeout = versusOptions.CommandTimeout;

            services.AddDbContextPool<SiteDbContext>(
                options => options
                            .UseSqlServer(
                                    versusOptions.ConnectionString,

                                    // Retry with some safe SQL exceptions
                                    x => x.EnableRetryOnFailure()

                                    // Set assembly with DbContext classes
                                    .MigrationsAssembly("VERSUS.Infrastructure")

                                    //Set command timeout
                                    .CommandTimeout(versusOptions.CommandTimeout.Seconds))

                            // Throw an exception if there is an issue converting LINQ to database calls
                            .ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning))

                            // Log sensitive error data in development
                            .EnableSensitiveDataLogging(env.IsDevelopment())

                            // Turn off tracking by default
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking))

                     .AddScoped<IReviewService, ReviewService>()

                     .AddSignalR();

            return services;
        }

        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentityCore<SiteUser>()
                .AddRoles<SiteRole>()
                .AddEntityFrameworkStores<SiteDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
            services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, configuration.GetSection("CookieAuthenticationOptions"));

            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies(o => { });

            return services;
        }
    }
}