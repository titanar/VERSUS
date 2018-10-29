using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using VERSUS.Core;

namespace VERSUS.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        /// <summary>
        /// Application settings.
        /// </summary>
        public static IOptions<VersusOptions> VersusOptions { get; set; }

        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment env)
        {
            services.AddDbContextPool<DbContext>(
                options => options
                            .UseSqlServer(
                                    VersusOptions.Value.ConnectionString,

                                    // Retry with some safe SQL exceptions
                                    x => x.EnableRetryOnFailure()

                                    //Set command timeout
                                    .CommandTimeout(VersusOptions.Value.CommandTimeout))

                            // Throw an exception if there is an issue converting LINQ to database calls
                            .ConfigureWarnings(x => x.Throw(RelationalEventId.QueryClientEvaluationWarning))

                            // Log sensitive error data in development
                            .EnableSensitiveDataLogging(env.IsDevelopment())

                            // Turn off tracking by default
                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            return services;
        }
    }
}
