using System;
using KenticoCloud.Delivery;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using React.AspNet;

namespace VERSUS.App
{
	public class Startup
	{
		private const string ErrorHandlingPath = "/Site/Error";

		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; }

		// Add services to the container. This method is called by the runtime.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMemoryCache();

			services.AddOptions();

			services.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
				options.HttpOnly = HttpOnlyPolicy.Always;
				options.Secure = CookieSecurePolicy.SameAsRequest;
			});

			services.Configure<DeliveryOptions>(Configuration);

			// Singleton instances of Delivery client and resolver classes
			services.AddSingleton<IDeliveryClient, DeliveryClient>();
			services.AddSingleton<IContentLinkUrlResolver, VersusContentLinkUrlResolver>();
			services.AddSingleton<ICodeFirstTypeProvider, VersusTypeProvider>();
			//services.AddSingleton<IInlineContentItemsProcessor, VersusInlineContentItemsProcessor>();

			// Required ReactJS services
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddReact();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			return services.BuildServiceProvider();
		}

		// Configure the HTTP request pipeline. The order of these methods matters. This method is called by the runtime.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseHttpsRedirection();

			app.UseStatusCodePagesWithReExecute(ErrorHandlingPath + "/{0}");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();

				// Initialise ReactJS.NET. Must be before static files.
				app.UseReact(config =>
				{
					// If you want to use server-side rendering of React components,
					// add all the necessary JavaScript files here. This includes
					// your components as well as all of their dependencies.
					config
						.AddScript("~/js/versus.jsx")
						;
				});
			}
			else
			{
				app.UseExceptionHandler(ErrorHandlingPath);
				app.UseHsts();

				// Initialise ReactJS.NET. Must be before static files.
				app.UseReact(config =>
				{
				// Improve performance by
				// disabling ReactJS.NET's version of Babel and loading the
				// pre-transpiled scripts.
				config
					.SetLoadBabel(false)
					.AddScriptWithoutTransform("~/js/versus.min.js")
					;
				});
			}


			app.UseStaticFiles();
			app.UseCookiePolicy();

			app.UseMvc(routes => MapRoutes(routes));
		}

		// Map MVC routes
		private static void MapRoutes(IRouteBuilder routes)
		{
			routes.MapRoute(name: "error",
							template: "Site/Error/{errorCode?}",
							defaults: new { controller = "Site", action = "Error"});

			routes.MapRoute(name: "default",
							template: "/",
							defaults: new { controller = "Site", action = "Index" });
		}
	}
}
