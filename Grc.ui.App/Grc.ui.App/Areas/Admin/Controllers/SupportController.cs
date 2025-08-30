using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http;
using Grc.ui.App.Http.Responses;
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
        private readonly ISystemActivityService _activityService;
       
        
        public SupportController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService,
                                 ISupportDashboardFactory dDashboardFactory,
                                 IErrorService errorService,
                                 ISystemActivityService activityService,
                                 IGrcErrorFactory errorFactory,
                                 SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, 
                  errorService, errorFactory, sessionManager) {
            _accessService = accessService;
            _authService = authService;
            _dDashboardFactory = dDashboardFactory;
            _activityService = activityService;
        }

        [LogActivityResult("User Login", "User logged in to the system", ActivityTypeDefaults.USER_LOGIN, "SystemUser")]
        public async Task<IActionResult> Index(){
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    return HandleLoginErrors(grcResponse.Error.Message, model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareAdminDashboardModelAsync(currentUser);
            } catch(Exception ex){ 
                Logger.LogActivity($"Username validation error: {ex.Message}", "ERROR");
                return HandleLoginErrors(LocalizationService.GetLocalizedLabel("Error.Service.Unavailable"), model);
            }

            return View(model);
        }

        [HttpPost("support/activities/allActivities")]
        public async Task<IActionResult> AllActivities([FromBody] TableListRequest request) {

            //..get user IP address
            var ipAddress = WebHelper.GetCurrentIpAddress();

            //..get current authenticated user record
            var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (grcResponse.HasError) {
                    Logger.LogActivity($"ACTIVITY DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
            }

            //..update with user data
            var currentUser = grcResponse.Data;
            request.UserId = currentUser.UserId;
            request.IPAddress = ipAddress;
            request.PageSize = 7;

            //..get list of a activity logs
            var activitydata = await _activityService.GetActivityLogsAsync(request);

            PagedResponse<ActivityModel> activityList = new ();
            if(activitydata.HasError){ 
                Logger.LogActivity($"ACTIVITY DATA ERROR: Failed to retrieve activity log items - {JsonSerializer.Serialize(activitydata)}");
            } else {
                activityList = activitydata.Data;
                Logger.LogActivity($"ACTIVITY DATA ERROR: Failed to retrieve activity log items - {JsonSerializer.Serialize(activityList)}");
            }

            activityList.Entities ??= new();
            return Ok(new {
                data = activityList.Entities,
                recordsTotal = activityList.TotalCount ,
                recordsFiltered = activityList.TotalCount 
            });
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
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> PermissionSets() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> AssignPermissions() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> PermissionDelegation() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> Bugs() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> NewBugs() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> BugFixes() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }
        
        public async Task<IActionResult> UserReportedBugs() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> ActivityRecord() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        public async Task<IActionResult> SendEmail() {
            string error_msg = "Active users exception occurred";
            string error_source = $"SUPPORT CONTROLLER - ActiveUsers";
            string error_stacktrace = $"Error details";
            _= await ProcessErrorAsync(error_msg, error_source, error_stacktrace);
            return View();
        }

        [HttpPost]
        [LogActivityResult("User Logout", "User logged out of the system", ActivityTypeDefaults.USER_LOGOUT, "SystemUser")]
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
