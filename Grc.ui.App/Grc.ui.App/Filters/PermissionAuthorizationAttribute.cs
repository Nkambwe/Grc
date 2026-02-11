using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace Grc.ui.App.Filters {
    /// <summary>
    /// Restricts access based on specific permissions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class PermissionAuthorizationAttribute : Attribute, IAuthorizationFilter {
        private readonly string[] _requiredPermissions;
        private readonly bool _requireAll;

        /// <param name="requireAll">If true, user must have ALL permissions. If false, user needs ANY permission.</param>
        public PermissionAuthorizationAttribute(bool requireAll = false, params string[] requiredPermissions) {
            _requiredPermissions = requiredPermissions ?? Array.Empty<string>();
            _requireAll = requireAll;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            var user = context.HttpContext.User;

            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true) {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return;
            }

            // Get user permissions from claims
            var permissionsClaim = user.FindFirst("Permissions")?.Value;
            if (string.IsNullOrEmpty(permissionsClaim)) {
                context.Result = new ForbidResult();
                return;
            }

            List<string> userPermissions;
            try {
                userPermissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim) ?? new List<string>();
            } catch {
                userPermissions = new List<string>();
            }

            //..check permissions
            bool hasAccess = _requireAll
                ? _requiredPermissions.All(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase))
                : _requiredPermissions.Any(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));

            if (!hasAccess) {
                //..log unauthorized access attempt
                var logger = context.HttpContext.RequestServices.GetService<ILogger<PermissionAuthorizationAttribute>>();
                logger?.LogWarning($"Permission denied for user {user.Identity.Name}. Required: {string.Join(", ", _requiredPermissions)}");

                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "" });
                return;
            }
        }
    }
}
