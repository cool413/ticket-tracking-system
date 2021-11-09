using Microsoft.AspNetCore.Builder;
using Services.Infrastructure.Exceptions;

namespace Services.Infrastructure.Extensions
{
    public static class ExceptionMiddlewareExtension
    {
        public static void UseCustomExceptionMiddleware(this IApplicationBuilder app
           )
        {
            app.UseMiddleware<CustomExceptionMiddleware>();
        }
    }
}
