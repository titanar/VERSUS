using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace VERSUS.Infrastructure.Middleware
{
    public class ExceptionClearResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionClearResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //try
            //{
            await _next(context);
            //}
            //catch (Exception ex)
            //{
            //    context.Response.Redirect("/Site/Error");
            //}
        }
    }
}