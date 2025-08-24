using Grc.ui.App.Utils;
using Grc.ui.App.Extensions.Http;
using System.Security.Claims;
using Grc.ui.App.Services;
using Grc.ui.App.Infrastructure;

namespace Grc.ui.App.Middleware {

    /// <summary>
    /// Middleware class to register SessionManager and monitor session activity
    /// </summary>
    public class SessionMiddleware {

        private readonly RequestDelegate _next;
        private readonly TimeSpan _sessionTimeout;

        public SessionMiddleware(RequestDelegate next, TimeSpan sessionTimeout) {
            _next = next;
            _sessionTimeout = sessionTimeout;
        }

        public async Task InvokeAsync(HttpContext context, SessionManager sessionManager, IWebHelper webHelper) {

            //..skip session checks for static files and API endpoints that don't need session
            if (context.Request.Path.StartsWithSegments("/api") || context.Request.Path.StartsWithSegments("/static")) {
                await _next(context);
                return;
            }

            if (context.User.Identity.IsAuthenticated) {

                //..update last activity timestamp
                sessionManager.UpdateLastActivity();

                //..where workspace doesn't exist in session but user is authenticated, rebuild the workspace
                var workspace = sessionManager.GetWorkspace();
                if (workspace == null) {
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                     if (!string.IsNullOrEmpty(userId) && long.TryParse(userId, out long userIdLong)) {
                        //..get user IP address
                        var ipAddress = webHelper.GetCurrentIpAddress();

                        //..build workspace
                        var workspaceService = context.RequestServices.GetRequiredService<IWorkspaceService>();
                        workspace = await workspaceService.BuildWorkspaceAsync(userIdLong, ipAddress);
                        sessionManager.SetWorkspace(workspace);
                     }
                } else {
                    //.. we need to check for session timeout
                    var lastActivity = sessionManager.GetLastActivity();
                    if (lastActivity.HasValue && DateTime.UtcNow - lastActivity.Value > _sessionTimeout) {
                        //..where session has timed out, clear session and redirect to login
                        sessionManager.Clear();
                        context.Response.Redirect("/Login/UserLogin?timeout=true");
                        return;
                    }
                }
            }

            await _next(context);

        }
    }
}
