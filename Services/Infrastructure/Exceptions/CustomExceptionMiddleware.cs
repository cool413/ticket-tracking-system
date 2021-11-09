using System;
using System.Threading.Tasks;
using FunCity.Services.Infrastructure.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Services.Infrastructure.Exceptions
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(RequestDelegate next,
            ILogger<CustomExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (ApplicationException aex)
            {
                _logger.LogWarning(aex, $"{aex.Message}.");
                await CommonUtil.HandleExceptionAsync(context, aex, System.Net.HttpStatusCode.BadRequest).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Message:{ex.Message}, callStack:{ex.StackTrace}.");

                await CommonUtil.HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }
    }
}
