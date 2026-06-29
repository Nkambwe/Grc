using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Json;

namespace Grc.ui.App.Infrastructure.MvcHelpers {

    [HtmlTargetElement(Attributes = "require-permission")]
    public class PermissionTagHelper : TagHelper {
        [HtmlAttributeName("require-permission")]
        public string RequiredPermission { get; set; }

        [ViewContext]
        public ViewContext ViewContext { get; set; }

        private readonly SessionManager _sessionManager;
        private readonly IApplicationLogger _logger;

        public PermissionTagHelper(SessionManager sessionManager, IApplicationLoggerFactory loggerFactory) {
            _sessionManager = sessionManager;
            _logger = loggerFactory.CreateLogger();
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            //..ensure session is loaded for this request
            await ViewContext.HttpContext.Session.LoadAsync();

            var permissions = _sessionManager.Get<List<string>>("Permissions")
                ?? new List<string>();

            _logger.LogActivity($"[TagHelper] RequiredPermission={RequiredPermission}, " +
                               $"PermissionCount={permissions.Count}, " +
                               $"SessionId={ViewContext.HttpContext.Session.Id}");

            if (!permissions.Any(p => string.Equals(
                    p?.Trim(),
                    RequiredPermission?.Trim(),
                    StringComparison.OrdinalIgnoreCase))) {
                output.SuppressOutput();
            }
        }

        //public override void Process(TagHelperContext context, TagHelperOutput output) {
        //    var permissions = _sessionManager.Get<List<string>>("Permissions")?? new List<string>();
        //    if (!permissions.Any()) {
        //        var workerPerms = _sessionManager.GetWorkspace();
        //        if (workerPerms != null) {
        //            var lst = workerPerms.Permissions;
        //            if (lst != null) {
        //                permissions = lst.ToList();
        //                _logger.LogActivity($"Permissions FROM WORKERSPACE SESSION : [{string.Join(", ", permissions)}]");
        //            }
        //        }
        //    }
        //    _logger.LogActivity($"Permissions: [{string.Join(", ", permissions)}]");
        //    if (!permissions.Any(p =>string.Equals(p?.Trim(), RequiredPermission?.Trim(),StringComparison.OrdinalIgnoreCase))) {
        //        output.SuppressOutput();
        //    }
        //}
    }
}
