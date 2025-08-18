using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace Grc.ui.App.Areas.Admin.Controllers {

    [Area("Admin")]
    public class SupportController : AdminBaseController {
        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;
        private readonly ISupportDashboardFactory _dDashboardFactory;
        public SupportController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService,
                                 ISupportDashboardFactory dDashboardFactory) 
            : base(loggerFactory, environment, webHelper, localizationService) {
           _accessService = accessService;
            _authService = authService;
            _dDashboardFactory = dDashboardFactory;
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
                currentUser.LastLoginIpAddress = ipAddress;
                model = await _dDashboardFactory.PrepareAdminDashboardModelAsync(currentUser);
                model.TotalUsers = (await _accessService.CountAllUsersAsync(currentUser.UserId, ipAddress)).Count;
                model.ActiveUsers = (await _accessService.CountActiveUsersAsync(currentUser.UserId, ipAddress)).Count;
            } catch(Exception ex){ 
                Logger.LogActivity($"Username validation error: {ex.Message}", "ERROR");
                return HandleLoginErrors(LocalizationService.GetLocalizedLabel("Error.Service.Unavailable"), model);
            }

            return View(model);
        }

        public async Task<IActionResult> Departments() {
            return View();
        }

        public async Task<IActionResult> Users() {
            return View();
        }
        
        public async Task<IActionResult> Roles() {
             return View();
        }

        public async Task<IActionResult> PermissionSets() {
            return View();
        }
        
        public async Task<IActionResult> AssignPermissions() {
            return View();
        }

        public async Task<IActionResult> PermissionDelegation() {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Logout() {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.Identity?.Name;
                Logger.LogActivity($"Admin user logging out: {username}", "INFO");
        
                // Update logged_in status in database before signing out
                long id = 0;
                if (!string.IsNullOrEmpty(userId)) {
                    _ = long.TryParse(userId, out id);
                    await _accessService.UpdateLoggedInStatusAsync(id, false, ipAddress);
                }
        
                //..sign out from cookie authentication
                await _authService.SignOutAsync(new LogoutModel(){UserId = id, IPAddress = ipAddress});
        
                // Return JSON response for AJAX
                return Json(new { 
                    success = true, 
                    redirectUrl = Url.Action("Login", "Application", new { area = "" }),
                    message = "Logged out successfully"
                });
        
            } catch (Exception ex) {
                Logger.LogActivity($"Error during admin logout: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");
        
                return Json(new { 
                    success = false, 
                    message = LocalizationService.GetLocalizedLabel("Error.Occurance"),
                    error = new { message = "Logout failed. Please try again." }
                });
            }
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
