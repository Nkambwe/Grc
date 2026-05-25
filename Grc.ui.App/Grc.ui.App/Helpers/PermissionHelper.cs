using Grc.ui.App.Utils;

namespace Grc.ui.App.Helpers {
    public static class PermissionHelper {
        public static List<string> GetPermissions(HttpContext context) {
            var sessionManager = context.RequestServices.GetRequiredService<SessionManager>();
            return sessionManager.Get<List<string>>("Permissions") ?? new List<string>();
        }
    }
}
