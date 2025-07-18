using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class GrcBaseController : Controller  {

        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;
        protected readonly IWebHelper WebHelper;

        public GrcBaseController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment,
                                 IWebHelper webHelper) {
            Logger = loggerFactory.CreateLogger("app_controllers");
            Environment = environment;
            WebHelper = webHelper;
        }

        protected string GetAntiForgeryToken() {
            var antiforgery = HttpContext.RequestServices.GetRequiredService<IAntiforgery>();
            var tokenSet = antiforgery.GetAndStoreTokens(HttpContext);
            return tokenSet.RequestToken;
        }

        public void Notify(string message, string title = "GRC NOTIFICATION", NotificationType type=NotificationType.Success) {
            var notificationMessage = new NotificationMessage() {
                Title = title,
                Message = message,
                Icon = type.GetEnumMemberValue(),
                Type  = type.GetEnumMemberValue()
            };

            TempData["Message"] = JsonSerializer.Serialize(notificationMessage); 
        }
    }

}
