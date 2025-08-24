using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Diagnostics;
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
                                 ISupportDashboardFactory dDashboardFactory,
                                 IErrorService errorService,
                                 IGrcErrorFactory errorFactory,
                                 SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, 
                  errorService, errorFactory, sessionManager) {
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
                var stats = (await _accessService.StatisticAsync(currentUser.UserId, ipAddress));
                model.TotalUsers = stats.TotalUsers;
                model.ActiveUsers = stats.ActiveUsers;
                model.DeactivatedUsers= stats.DeactivatedUsers;
                model.UnApprovedUsers= stats.UnApprovedUsers;
                model.UnverifiedUsers = stats.UnverifiedUsers;
                model.DeletedUsers= stats.DeletedUsers;
                model.TotalBugs = stats.TotalBugs;
                model.NewBugs = stats.NewBugs;
                model.BugFixes = stats.BugFixes;
                model.BugProgress = stats.BugProgress;
                model.UserReportedBugs = stats.UserReportedBugs;

            } catch(Exception ex){ 
                Logger.LogActivity($"Username validation error: {ex.Message}", "ERROR");
                return HandleLoginErrors(LocalizationService.GetLocalizedLabel("Error.Service.Unavailable"), model);
            }

            return View(model);
        }

        public async Task<IActionResult> Departments() {;
            string error_msg = "Department exception occurred";
            string error_source = $"SUPPORT CONTROLLER - Departments";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> Users() {
            string error_msg = "Users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - Users";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> ActiveUsers() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        public async Task<IActionResult> DisabledUsers() {
            string error_msg = "DisabledUsers exception occurred";
            string error_source = $"SUPPORT CONTROLLER - Users";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> UnapprovedUsers() {
            string error_msg = "Unapproved users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - UnapprovedUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> Roles() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> PermissionSets() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> AssignPermissions() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> PermissionDelegation() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> Bugs() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> NewBugs() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> BugFixes() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> UserReportedBugs() {
            var ex = HttpContext.Features.Get<IExceptionHandlerFeature>();
            string error_msg = ex.Error.Message;
            string error_source = $"SUPPORT CONTROLLER - {ex.Error.Source}";
            string error_stacktrace = $"{ex.Error.StackTrace}";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
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
        
                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "SUPPORTCONTROLLER-LOGOUT", ex.StackTrace);
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
