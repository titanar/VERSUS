using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;

using Microsoft.Extensions.DependencyInjection;

using React.AspNet;

namespace VERSUS.Core.Extensions
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddReactServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor()
                    .AddReact()
                    .AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
                    .AddChakraCore();

            return services;
        }
    }
}