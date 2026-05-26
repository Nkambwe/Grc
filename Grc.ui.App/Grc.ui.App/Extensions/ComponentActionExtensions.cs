using Grc.ui.App.Helpers;
using Grc.ui.App.Utils;
using System.Security.Claims;

namespace Grc.ui.App.Extensions {
    public static class ComponentActionExtensions {

        //public static List<ComponentAction> AddIfPermission(this List<ComponentAction> actions, ClaimsPrincipal user, 
        //    string permission, params ComponentAction[] actionsToAdd) {
        public static List<ComponentAction> AddIfPermission(this List<ComponentAction> actions, HttpContext context, string permission, params ComponentAction[] actionsToAdd) {


            //var permissionsClaim = user.FindFirst("Permissions")?.Value;
            //if (string.IsNullOrEmpty(permissionsClaim))
            //    return actions;

            //var permissions = System.Text.Json.JsonSerializer.Deserialize<List<string>>(permissionsClaim) ?? new List<string>();
            var permissions = context.RequestServices.GetRequiredService<SessionManager>().Get<List<string>>("Permissions") ?? new List<string>();
            if (permissions.Contains(permission, StringComparer.OrdinalIgnoreCase)) {
                actions.AddRange(actionsToAdd);
            }
        
            return actions;
        }
    }

}
