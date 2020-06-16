using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Rox.WebSample
{
    public class GlobalLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalLoggerMiddleware(RequestDelegate next, ILogger<GlobalLoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            _logger.LogInformation($"[RequestAt]{httpContext.Request.Path}");
            return _next(httpContext);
        }
    }

    public static class GlobalLoggerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalLoggerMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalLoggerMiddleware>();
        }
    }
}
