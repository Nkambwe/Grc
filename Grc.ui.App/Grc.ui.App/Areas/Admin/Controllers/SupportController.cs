using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Areas.Admin.Controllers {

    [Area("Admin")]
    public class SupportController : AdminBaseController {
        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;
        public SupportController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService) 
            : base(loggerFactory, environment, webHelper, localizationService) {
           _accessService = accessService;
            _authService = authService;
        }

        public async Task<IActionResult> Index(){
            var currentUser = await _authService.GetCurrentUserAsync();
            
            var model = new AdminDashboardModel{
                WelcomeMessage = $"{LocalizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!",
                TotalUsers = await _accessService.CountAllUsersAsync(),
                ActiveUsers = await _accessService.CountActiveUsersAsync(),
                LastLogin = DateTime.UtcNow
            };

            return View(model);
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
