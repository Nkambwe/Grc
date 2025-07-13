using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class GrcBaseController : Controller  {

        protected readonly IApplicationLogger Logger;
        protected readonly IEnvironmentProvider Environment;

        public GrcBaseController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment) {
            Logger = loggerFactory.CreateLogger("app_controllers");
            Environment = environment;
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
