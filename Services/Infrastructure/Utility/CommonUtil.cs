using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace FunCity.Services.Infrastructure.Utility
{
    public static class CommonUtil
    {
        public static Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            const string ContentType = "application/json";
            context.Response.ContentType = ContentType;
            var result = JsonConvert.SerializeObject(new
            {
                StatusCode = statusCode,
                Msg = exception.Message
            });

            context.Response.ContentType = ContentType;
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(result);
        }

        public static string GetCurrentController(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var endpoint = context.GetEndpoint();
            if (endpoint is RouteEndpoint routeEndpoint)
            {
                return routeEndpoint.RoutePattern.Defaults["controller"].ToString();
            }

            return string.Empty;
            //return endpoint?.Metadata.GetMetadata<EndpointNameMetadata>()?.EndpointName;
        }
    }
}
