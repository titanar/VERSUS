﻿using System;
using KenticoCloud.Delivery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using React.AspNet;
using VERSUS.App.Resolvers;
using VERSUS.Core;
using VERSUS.Core.Extensions;
using VERSUS.Infrastructure.Extensions;
using VERSUS.Infrastructure.Middleware;
using VERSUS.Kentico.Extensions;

namespace VERSUS.App
{
    public class Startup
	{
		public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			Configuration = configuration;
            Environment = env;
		}

		// Add services to the container. This method is called by the runtime.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
            services.AddOptions()

                    .Configure<VersusOptions>(Configuration)

                    .AddDatabaseContext(Configuration, Environment)

                    .AddMemoryCache()

                    .Configure<CookiePolicyOptions>(Configuration)
                    .PostConfigure<CookiePolicyOptions>(options =>
                    {
                        // This lambda determines whether user consent for non-essential cookies is needed for a given request
                        options.CheckConsentNeeded = context => true;
                    })

                    .AddSingleton<IContentLinkUrlResolver>(_ => new VersusContentLinkUrlResolver())

                    .AddKenticoDelivery(Configuration)

                    .AddReactServices()			

			        .AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

			return services.BuildServiceProvider();
		}

		// Configure the HTTP request pipeline. The order of these methods matters. This method is called by the runtime.
		public void Configure(IApplicationBuilder app, IOptions<VersusOptions> options)
		{
			app.UseHttpsRedirection()
				.UseStatusCodePagesWithReExecute(options.Value.ErrorHandlingRoute + "/{0}");

			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(options.Value.ErrorHandlingRoute)
				   .UseHsts();
			}

			app.UseMiddleware<ExceptionClearResponseMiddleware>();

			// Initialise ReactJS.NET. Must be before static files.
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
