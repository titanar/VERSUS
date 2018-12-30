using System;

using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using React.AspNet;

using VERSUS.Core;
using VERSUS.Core.Extensions;
using VERSUS.Infrastructure.Extensions;
using VERSUS.Kentico.Extensions;
using VERSUS.Kentico.Webhooks.Extensions;

namespace VERSUS.App
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Environment = env;
        }

        /// <summary>
        /// Add services to the container. This method is called by the runtime.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<VersusOptions>(Configuration)

                    .AddDatabaseContext(Configuration, Environment)

                    .AddCookiePolicy(Configuration)
                    .AddIdentity(Configuration)

                    .AddKenticoDelivery(Configuration)
                    .AddKenticoWebhooks();

            services.AddHttpContextAccessor()
                    .AddReact()
                    .AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
                    .AddChakraCore();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            return services.BuildServiceProvider();
        }

        /// <summary>
        /// Configure the HTTP request pipeline. The order of these methods matters. This method is called by the runtime.
        /// </summary>
        public void Configure(IApplicationBuilder app, IOptionsSnapshot<VersusOptions> versusOptions, IOptionsSnapshot<KenticoOptions> kenticoOptions)
        {
            app.UseHttpsRedirection()
                .UseWebhookMiddleware(kenticoOptions.Value.KenticoCloudWebhookEndpoint)
                .UseStatusCodePagesWithReExecute($"{versusOptions.Value.ErrorHandlingRoute}/{{0}}")
                .UseSignalR();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(versusOptions.Value.ErrorHandlingRoute)
                   .UseHsts();
            }

            app.UseAuthentication();
            //app.UseMiddleware<ExceptionClearResponseMiddleware>();

            // Initialize ReactJS.NET. Must be before static files.
            app.UseReact(config =>
                {
                    // Server-side rendering of React components. Babel and React are provided manually.
                    config
                        .SetReuseJavaScriptEngines(true)
                        .SetLoadBabel(false)
                        .SetLoadReact(false)
                        .AddScriptWithoutTransform("/js/versus.js");
                })

                .UseStaticFiles()
                .UseCookiePolicy()

                .UseMvc();
        }
    }
}