using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;

namespace Grc.ui.App.Infrastructure.MvcHelpers {
    [HtmlTargetElement("div", Attributes = "require-permission")]
    [HtmlTargetElement("a", Attributes = "require-permission")]
    [HtmlTargetElement("button", Attributes = "require-permission")]
    public class PermissionTagHelper : TagHelper {
        [HtmlAttributeName("require-permission")]
        public string RequiredPermission { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var user = ViewContext.HttpContext.User;
            var permissionsClaim = user.FindFirst("Permissions")?.Value;

            if (string.IsNullOrEmpty(permissionsClaim)) {
                output.SuppressOutput();
                return;
            }

            var permissions = JsonSerializer.Deserialize<List<string>>(permissionsClaim) ?? new List<string>();

            if (!permissions.Contains(RequiredPermission, StringComparer.OrdinalIgnoreCase)) {
                output.SuppressOutput();
            }
        }
    }
}
