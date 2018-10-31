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
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, IHostingEnvironment env)
        {
            var versusOptions = services.BuildServiceProvider().GetRequiredService<IOptions<VersusOptions>>().Value;

            services.AddDbContextPool<DbContext>(
                options => options
                            .UseSqlServer(
                                    versusOptions.ConnectionString,

                                    // Retry with some safe SQL exceptions
                                    x => x.EnableRetryOnFailure()

                                    //Set command timeout
                                    .CommandTimeout(versusOptions.CommandTimeout))

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
