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

        // Add services to the container. This method is called by the runtime.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.Configure<VersusOptions>(Configuration)

                    .AddDatabaseContext(Configuration, Environment)

                    .Configure<CookiePolicyOptions>(Configuration)
                    .PostConfigure<CookiePolicyOptions>(options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request
                        options.CheckConsentNeeded = context => true;
                    })

                    .AddKenticoDelivery(Configuration)

                    .AddKenticoWebhooks();

            services.AddHttpContextAccessor()
                    .AddReact()
                    .AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
                    .AddChakraCore();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            return services.BuildServiceProvider();
        }

        // Configure the HTTP request pipeline. The order of these methods matters. This method is called by the runtime.
        public void Configure(IApplicationBuilder app, IOptionsSnapshot<VersusOptions> options)
        {
            app.UseHttpsRedirection()
                .UseWebhookMiddleware(options.Value.KenticoCloudWebhookEndpoint)
                .UseStatusCodePagesWithReExecute($"{options.Value.ErrorHandlingRoute}/{{0}}")
                .UseSignalR();

            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(options.Value.ErrorHandlingRoute)
                   .UseHsts();
            }

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