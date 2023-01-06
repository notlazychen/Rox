using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Barbecue
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if(!httpContext.Request.Cookies.TryGetValue("user_id", out var userId))
            {
                userId = Guid.NewGuid().ToString();
                httpContext.Response.Cookies.Append("user_id", userId);
            }
            httpContext.User = new System.Security.Claims.ClaimsPrincipal(new ClaimsIdentity(new List<Claim>() 
            { 
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId),
            }));
            await _next(httpContext);
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
