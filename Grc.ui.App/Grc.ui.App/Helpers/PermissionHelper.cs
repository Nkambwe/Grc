using Grc.ui.App.Utils;
using System.Text.Json;
using Grc.ui.App.Extensions.Http;

namespace Grc.ui.App.Helpers {
    public static class PermissionHelper {
        public static List<string> GetPermissions(HttpContext context) {
            var sessionManager = context.RequestServices.GetRequiredService<SessionManager>();
            var loggerFactory = context.RequestServices.GetRequiredService<IApplicationLoggerFactory>();
            var logger = loggerFactory.CreateLogger();

            var permissions = sessionManager.Get<List<string>>("Permissions") ?? new List<string>();
            if (!permissions.Any()) {
                var workerPerms = sessionManager.GetWorkspace();
                if (workerPerms != null) {
                    var lst = workerPerms.Permissions;
                    if (lst != null) {
                        permissions = lst.ToList();
                        logger.LogActivity($"Permissions FROM WORKERSPACE SESSION : [{string.Join(", ", permissions)}]");
                    }
                }
            }

            logger.LogActivity($"WORKSPACE PERMISSIONS IN HELPER: {JsonSerializer.Serialize(permissions)}");

            return permissions;
        }
    }

}
