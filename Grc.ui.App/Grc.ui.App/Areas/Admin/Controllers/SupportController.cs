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
            var model = new AdminDashboardModel();
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    return HandleLoginErrors(grcResponse.Error.Message, model);
                }

                var currentUser = grcResponse.Data;
                model.WelcomeMessage = $"{LocalizationService.GetLocalizedLabel("App.Label.Welcome")}, {currentUser?.FirstName}!";
                model.TotalUsers = (await _accessService.CountAllUsersAsync(currentUser.Id, ipAddress)).Count;
                model.ActiveUsers = (await _accessService.CountActiveUsersAsync(currentUser.Id, ipAddress)).Count;

                model.QuickActions = new List<QuickActionModel> {
                    new() { Label = "Add User", IconClass = "mdi mdi-account-multiple-plus-outline", Url = "/Admin/User/Add" },
                    new() { Label = "New Dept", IconClass = "mdi mdi-share-all-outline", Url = "/Admin/Department/Add" },
                    new() { Label = "Assign Permissions", IconClass = "mdi mdi-shield-check-outline", Url = "/Admin/Permissions/Index" }
                    // Load from DB or user prefs in future
                };
                
                model.Recents = new List<RecentModel> {
                    new() { Label = "Add User", IconClass = "mdi mdi-account-multiple-plus-outline", Url = "/Admin/User/Add" },
                    new() { Label = "New Dept", IconClass = "mdi mdi-share-all-outline", Url = "/Admin/Department/Add" },
                    new() { Label = "Assign Permissions", IconClass = "mdi mdi-shield-check-outline", Url = "/Admin/Permissions/Index" },
                    new() { Label = "User Data", IconClass = "mdi mdi-account-details-outline", Url = "/Admin/User" },
                    new() { Label = "User Goups", IconClass = "mdi mdi-account-group-outline", Url = "/Admin/Reports" }
                    // Load from session
                };

                model.Favourites = new List<FavouriteModel> {
                    new() { Label = "User Data", IconClass = "mdi mdi-account-details-outline", Url = "/Admin/User" },
                    new() { Label = "User Groups", IconClass = "mdi mdi-account-group-outline", Url = "/Admin/Reports" }
                };


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
