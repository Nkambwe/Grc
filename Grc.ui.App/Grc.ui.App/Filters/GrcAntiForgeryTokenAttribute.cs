using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Grc.ui.App.Filters {

    /// <summary>
    /// GRC Custom filter class to implement ant-forgery token protection against cross-site request forgery (CSRF) attacks. 
    /// </summary>
    public class GrcAntiForgeryTokenAttribute: ActionFilterAttribute, IAuthorizationFilter {
        
        private readonly IAntiforgery _antiforgery;

        public GrcAntiForgeryTokenAttribute(IAntiforgery antiforgery) {
            _antiforgery = antiforgery;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            if (context.HttpContext.Request.Method == "POST" || context.HttpContext.Request.Method == "PUT" || context.HttpContext.Request.Method == "DELETE") {
                try {
                    _antiforgery.ValidateRequestAsync(context.HttpContext).Wait();
                } catch (AntiforgeryValidationException) {
                    context.Result = new BadRequestResult();
                }
            }
        }
    }
}
