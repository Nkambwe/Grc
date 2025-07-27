using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
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
            var model = new AdminDashboardModel();
            try {
                var grcResponse = await _authService.GetCurrentUserAsync();
                if (grcResponse.HasError) {
                    return HandleLoginErrors(grcResponse.Error.Message, model);
                }

                var currentUser = grcResponse.Data;
                model.WelcomeMessage = $"{LocalizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!";
                model.TotalUsers = await _accessService.CountAllUsersAsync();
                model.ActiveUsers = await _accessService.CountActiveUsersAsync();
                model.LastLogin = DateTime.UtcNow;
            } catch(Exception ex){ 
                Logger.LogActivity($"Username validation error: {ex.Message}", "ERROR");
                return HandleLoginErrors(LocalizationService.GetLocalizedLabel("Error.Service.Unavailable"), model);
            }

            return View(model);
        }

        private IActionResult HandleLoginErrors(string error, AdminDashboardModel model) {
            //..TODO redirect to login
            return View(model);
        }

        public void Notify(string message, string title = "GRC NOTIFICATION", NotificationType type = NotificationType.Success) {
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
