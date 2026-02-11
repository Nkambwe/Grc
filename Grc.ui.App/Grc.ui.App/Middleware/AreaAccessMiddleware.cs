using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Middleware {

    public class AreaAccessMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<AreaAccessMiddleware> _logger;

        public AreaAccessMiddleware(RequestDelegate next, ILogger<AreaAccessMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            var endpoint = context.GetEndpoint();
            var area = endpoint?.Metadata.GetMetadata<AreaAttribute>()?.RouteValue;

            if (string.IsNullOrEmpty(area)) {
                await _next(context);
                return;
            }

            var user = context.User;

            if (user.Identity?.IsAuthenticated != true) {
                await _next(context);
                return;
            }

            var roleGroup = user.FindFirst("RoleGroup")?.Value?.ToUpperInvariant();

            if (area == "Admin" && !IsAdminGroup(roleGroup)) {
                await HandleForbidden(context, "Admin");
                return;
            }

            if (area == "Operations" && !IsOperationsGroup(roleGroup)) {
                await HandleForbidden(context, "Operations");
                return;
            }

            await _next(context);
        }

        private async Task HandleForbidden(HttpContext context, string area) {
            _logger.LogWarning($"Blocked unauthorized {area} area access attempt by {context.User.Identity?.Name}");

            if (IsAjaxRequest(context.Request)) {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            context.Response.Redirect("/application/access-denied");
            await Task.CompletedTask;
        }

        private static bool IsAjaxRequest(HttpRequest request) {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || request.Headers["Accept"].Any(h => h.Contains("application/json"));
        }

        private static bool IsAdminGroup(string roleGroup) =>
            new[] { "DEVELOPER", "SYSTEM", "ADMINSUPPORT", "ADMINISTRATOR", "APPLICATIONSUPPORT" }
                .Contains(roleGroup);

        private static bool IsOperationsGroup(string roleGroup) =>
            new[] { "OPERATIONSERVICES", "OPERATIONADMIN", "OPERATIONGUESTS" }
                .Contains(roleGroup);
    }

}
