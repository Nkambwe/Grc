using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Grc.ui.App.Filters {
    /// <summary>
    /// Restricts access to specific areas based on user's role group
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AreaAuthorizationAttribute : Attribute, IAsyncAuthorizationFilter {
        private readonly string[] _allowedRoleGroups;

        public AreaAuthorizationAttribute(params string[] allowedRoleGroups) {
            _allowedRoleGroups = allowedRoleGroups ?? Array.Empty<string>();
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

            var userRoleGroup = user.FindFirst("RoleGroup")?.Value;

            if (string.IsNullOrWhiteSpace(userRoleGroup)) {
                context.Result = new ForbidResult();
                return Task.CompletedTask;
            }

            if (_allowedRoleGroups.Length > 0 &&
                !_allowedRoleGroups.Any(rg => rg.Equals(userRoleGroup, StringComparison.OrdinalIgnoreCase))) {
                var logger = context.HttpContext.RequestServices
                    .GetService<ILogger<AreaAuthorizationAttribute>>();

                logger?.LogWarning($"Unauthorized access attempt by {user.Identity?.Name}");

                context.Result = RedirectToUserDashboard(userRoleGroup);
            }

            return Task.CompletedTask;
        }

        private static IActionResult RedirectToUserDashboard(string roleGroup) {
            if (IsAdminGroup(roleGroup)) {
                return new RedirectToActionResult("Index", "Support", new { area = "Admin" });
            } else if (IsOperationsGroup(roleGroup)) {
                return new RedirectToActionResult("Index", "OperationDashboard", new { area = "Operations" });
            } else if (IsComplianceGroup(roleGroup)) {
                return new RedirectToActionResult("Dashboard", "Application", new { area = "" });
            }

            return new RedirectToActionResult("AccessDenied", "Application", new { area = "" });
        }

        private static bool IsAdminGroup(string roleGroup) =>
            new[] { "DEVELOPER", "SYSTEM", "ADMINSUPPORT", "ADMINISTRATOR", "APPLICATIONSUPPORT" }
                .Contains(roleGroup?.ToUpper());

        private static bool IsOperationsGroup(string roleGroup) =>
            new[] { "OPERATIONSERVICES", "OPERATIONADMIN", "OPERATIONGUESTS" }
                .Contains(roleGroup?.ToUpper());

        private static bool IsComplianceGroup(string roleGroup) =>
            new[] { "COMPLIANCEDEPT", "COMPLIANCEADMIN", "COMPLIANCEGUESTS" }
                .Contains(roleGroup?.ToUpper());
    }
}
