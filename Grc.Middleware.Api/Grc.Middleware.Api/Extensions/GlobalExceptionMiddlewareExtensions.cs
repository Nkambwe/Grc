using Grc.Middleware.Api.Helpers;

namespace Grc.Middleware.Api.Extensions {

    public static class GlobalExceptionMiddlewareExtensions {
        public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder builder)
            => builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
