using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using React.AspNet;

using VERSUS.Infrastructure;
using VERSUS.Kentico;

namespace VERSUS.App
{
	public class Startup
	{
		private const string ErrorHandlingPath = "/Site/Error";

		public IConfiguration Configuration { get; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}


		// Add services to the container. This method is called by the runtime.
		public IServiceProvider ConfigureServices(IServiceCollection services)
		{
			services.AddMemoryCache()
				.AddOptions()
				.Configure<CookiePolicyOptions>(options =>
			{
				// This lambda determines whether user consent for non-essential cookies is needed for a given request
				options.CheckConsentNeeded = context => true;
				options.MinimumSameSitePolicy = SameSiteMode.None;
				options.HttpOnly = HttpOnlyPolicy.Always;
				options.Secure = CookieSecurePolicy.SameAsRequest;
			})
				.AddKenticoDelivery(Configuration)


			// Required ReactJS services
				.AddHttpContextAccessor()
				.AddReact()
				.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
				.AddChakraCore();

			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			return services.BuildServiceProvider();
		}

		// Configure the HTTP request pipeline. The order of these methods matters. This method is called by the runtime.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			app.UseHttpsRedirection()
				.UseStatusCodePagesWithReExecute(ErrorHandlingPath + "/{0}");

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler(ErrorHandlingPath)
				   .UseHsts();
			}

			app.UseMiddleware<ExceptionClearResponseMiddleware>();

			// Initialise ReactJS.NET. Must be before static files.
			app.UseReact(config =>
			{
				// If you want to use server-side rendering of React components,
				// add all the necessary JavaScript files here. This includes
				// your components as well as all of their dependencies.
				config
					.SetReuseJavaScriptEngines(true)
					.SetLoadBabel(false)
					.SetLoadReact(false)
					.AddScriptWithoutTransform("/js/versus.js");
			})

				.UseStaticFiles()
				.UseCookiePolicy()

				.UseMvc(routes => MapRoutes(routes));
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
