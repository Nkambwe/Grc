using Grc.ui.App.Middleware;

namespace Grc.ui.App.Extensions.Mvc {
    public static class ApplicationBuilderExtension {

        public static IApplicationBuilder UseApplicationSession(this IApplicationBuilder app, TimeSpan sessionTimeout)
            => app.UseMiddleware<SessionMiddleware>(sessionTimeout);

    }
}
