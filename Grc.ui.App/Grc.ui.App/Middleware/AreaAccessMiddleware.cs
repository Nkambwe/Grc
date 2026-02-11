namespace Grc.ui.App.Middleware {

    public class AreaAccessMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<AreaAccessMiddleware> _logger;

        public AreaAccessMiddleware(RequestDelegate next, ILogger<AreaAccessMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true && !string.IsNullOrEmpty(path))
            {
                var roleGroup = user.FindFirst("RoleGroup")?.Value?.ToUpper();

                // Check area access
                if (path.StartsWith("/admin") && !IsAdminGroup(roleGroup))
                {
                    _logger.LogWarning($"Blocked unauthorized Admin area access attempt by {user.Identity.Name}");
                    context.Response.Redirect("/Account/AccessDenied");
                    return;
                }

                if (path.StartsWith("/operations") && !IsOperationsGroup(roleGroup))
                {
                    _logger.LogWarning($"Blocked unauthorized Operations area access attempt by {user.Identity.Name}");
                    context.Response.Redirect("/Account/AccessDenied");
                    return;
                }
            }

            await _next(context);
        }

        private static bool IsAdminGroup(string roleGroup) =>
            new[] { "DEVELOPER", "SYSTEM", "ADMINSUPPORT", "ADMINISTRATOR", "APPLICATIONSUPPORT" }
                .Contains(roleGroup);

        private static bool IsOperationsGroup(string roleGroup) =>
            new[] { "OPERATIONSERVICES", "OPERATIONADMIN", "OPERATIONGUESTS" }
                .Contains(roleGroup);
    }
}
