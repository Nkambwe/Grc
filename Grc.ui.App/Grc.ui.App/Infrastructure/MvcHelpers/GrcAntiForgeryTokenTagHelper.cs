using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Grc.ui.App.Infrastructure.MvcHelpers {

    /// <summary>
    /// GRC TagHelper class that provides a more declarative way to include ant-forgery token in views
    /// </summary>
    /// <remarks>
    /// <grc-antiforgery-token></grc-antiforgery-token> -- Simple usage as Hidden input
    /// <grc-antiforgery-token as-meta="true"></grc-antiforgery-token>  -- use as meta tag in page header
    /// <grc-antiforgery-token auto-detect="true"></grc-antiforgery-token> -- checks ViewBag, 
    /// add  ViewBag.UseAjax = true; to endpoint This will make auto-detect render as meta
    /// </remarks>
    [HtmlTargetElement("grc-antiforgery-token")]
    public class GrcAntiForgeryTokenTagHelper : TagHelper {
        private readonly IAntiforgery _antiforgery;
        private readonly IActionContextAccessor _actionContextAccessor;

        /// <summary>
        /// Render as meta tag for AJAX (default: false = hidden input)
        /// </summary>
        [HtmlAttributeName("as-meta")]
        public bool AsMeta { get; set; } = false;

        /// <summary>
        /// Auto-detect based on ViewBag.UseAjax
        /// </summary>
        [HtmlAttributeName("auto-detect")]
        public bool AutoDetect { get; set; } = false;

        public GrcAntiForgeryTokenTagHelper(IAntiforgery antiforgery, IActionContextAccessor actionContextAccessor) {
            _antiforgery = antiforgery;
            _actionContextAccessor = actionContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            var httpContext = _actionContextAccessor.ActionContext?.HttpContext;
            if (httpContext == null) {
                output.SuppressOutput();
                return;
            }

            var tokenSet = _antiforgery.GetAndStoreTokens(httpContext);

            //..determine if we should render as meta in page header
            bool renderAsMeta = AsMeta;
        
            if (AutoDetect) {
                var viewContext = _actionContextAccessor.ActionContext as ViewContext;
                if (viewContext?.ViewBag.UseAjax == true) {
                    renderAsMeta = true;
                }
            }

            if (renderAsMeta) {
                output.TagName = "meta";
                output.Attributes.SetAttribute("name", "csrf-token");
                output.Attributes.SetAttribute("content", tokenSet.RequestToken);
            } else {
                output.TagName = "input";
                output.Attributes.SetAttribute("type", "hidden");
                output.Attributes.SetAttribute("name", "__RequestVerificationToken");
                output.Attributes.SetAttribute("value", tokenSet.RequestToken);
            }
        }
    }
}
