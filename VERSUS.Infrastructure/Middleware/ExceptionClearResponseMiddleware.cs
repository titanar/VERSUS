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
            // Set http status to 418? and set location header
            await _next(context);
            //}
            //catch (Exception ex)
            //{
            //    context.Response.Redirect("/Site/Error");
            //}
        }
    }
}