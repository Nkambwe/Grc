using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Grc.ui.App.Filters {
    /// <summary>
    /// Restricts access based on specific permissions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter {
        private readonly string[] _requiredPermissions;
        private readonly bool _requireAll;

        /// <summary>
        /// Attach permission to an endpoint
        /// </summary>
        /// <param name="requireAll">If true, user must have ALL permissions. If false, user needs ANY permission.</param>
        /// <param name="requiredPermissions"></param>
        public PermissionAuthorizationAttribute(bool requireAll = false, params string[] requiredPermissions) {
            _requiredPermissions = requiredPermissions ?? Array.Empty<string>();
            _requireAll = requireAll;
        }

        public Task OnAuthorizationAsync(AuthorizationFilterContext context) {
            // Proper AllowAnonymous detection
            var endpoint = context.HttpContext.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
                return Task.CompletedTask;

            var user = context.HttpContext.User;

            if (!user.Identity?.IsAuthenticated ?? true) {
                context.Result = new RedirectToActionResult("Login", "Application", new { area = "" });
                return Task.CompletedTask;
            }

            var permissionsClaim = user.FindFirst("Permissions")?.Value;

            if (string.IsNullOrEmpty(permissionsClaim)) {
                context.Result = new ForbidResult();
                return Task.CompletedTask;
            }

            List<string> userPermissions;

            try {
                userPermissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim) ?? new();
            } catch {
                userPermissions = new();
            }

            bool hasAccess = _requireAll
                ? _requiredPermissions.All(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase))
                : _requiredPermissions.Any(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));

            if (!hasAccess) {
                if (IsAjaxRequest(context.HttpContext.Request)) {
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                } else {
                    context.Result = new RedirectToActionResult("AccessDenied", "Application", new { area = "" });
                }     
            }

            return Task.CompletedTask;
        }

        private static bool IsAjaxRequest(HttpRequest request) {
            return request.Headers["X-Requested-With"] == "XMLHttpRequest"
                || request.Headers["Accept"].Any(h => h.Contains("application/json"));
        }


    }
}
