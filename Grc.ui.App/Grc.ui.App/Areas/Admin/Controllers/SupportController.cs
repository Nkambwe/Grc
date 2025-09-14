using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
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
        private readonly IDepartmentService _departmentService;
        private readonly IDepartmentFactory _departmentfactory;
        private readonly IBranchService _branchService;
        private readonly IDepartmentUnitService _departmentUnitService;
        
        public SupportController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService,
                                 ISupportDashboardFactory dDashboardFactory,
                                 IErrorService errorService,
                                 ISystemActivityService activityService,
                                 IDepartmentService departmentService,
                                 IDepartmentFactory departmentfactory,
                                 IGrcErrorFactory errorFactory,
                                 IDepartmentUnitService departmentUnitService,
                                 IBranchService branchService,
                                 SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, 
                  errorService, errorFactory, sessionManager) {
            _accessService = accessService;
            _authService = authService;
            _departmentfactory = departmentfactory;
            _departmentService = departmentService;
            _dDashboardFactory = dDashboardFactory;
            _activityService = activityService;
            _departmentUnitService = departmentUnitService;
            _branchService = branchService;
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
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareAdminDashboardModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> Departments() {
           var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);

                //..prepare deparmenet list model
                model.DepartmentListModel = await _departmentfactory.PrepareDepartmentListModelAsync(currentUser); 

            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> Users() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        
        public async Task<IActionResult> ActiveUsers() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> DisabledUsers() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        
        public async Task<IActionResult> UnapprovedUsers() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> Roles() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> PermissionSets() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        
        public async Task<IActionResult> AssignPermissions() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> PermissionDelegation() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        
        public async Task<IActionResult> Bugs() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        
        public async Task<IActionResult> NewBugs() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> BugFixes() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        
        public async Task<IActionResult> UserReportedBugs() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> ActivityRecord() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> SendEmail() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"SUPPORT-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
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

        #region Data actions

        [HttpGet("support/organization/getBranches")]
        public async Task<IActionResult> GetBranches() { 
            try {

                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                        Logger.LogActivity($"BRANCH LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                GrcRequest request = new() {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIEVEBRANCHES.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all branches
                var  branchData = await _branchService.GetBranchesAsync(request);

                List<BranchResponse> branches;
                if(branchData.HasError){ 
                    branches = new ();
                    Logger.LogActivity($"BRANCH DATA ERROR: Failed to retrieve branch items - {JsonSerializer.Serialize(branchData)}");
                } else {
                    branches = branchData.Data;
                    Logger.LogActivity($"BRANCH DATA - {JsonSerializer.Serialize(branches)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if(branches.Any()){ 
                    select2Data = branches.Select(branch => new {                        
                        id = branch.Id,
                        text = branch.BranchName 
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving branches: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }
        
        [HttpPost("support/organization/allBranches")]
        public async Task<IActionResult> AllBranches([FromBody] TableListRequest request) { 
            
            try{
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                        Logger.LogActivity($"BRANCH DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.PageSize = 7;

                //..get branch data
                var branchdata = await _branchService.GetAllBranchesAsync(request);

                PagedResponse<BranchResponse> branchList = new ();
                if(branchdata.HasError){ 
                    Logger.LogActivity($"BRANCH DATA ERROR: Failed to retrieve branch items - {JsonSerializer.Serialize(branchdata)}");
                } else {
                    branchList = branchdata.Data;
                    Logger.LogActivity($"BRANCH DATA - {JsonSerializer.Serialize(branchList)}");
                }

                branchList.Entities ??= new();
                return Ok(new {
                    data = branchList.Entities,
                    recordsTotal = branchList.TotalCount ,
                    recordsFiltered = branchList.TotalCount 
                });
            } catch(Exception ex){
                Logger.LogActivity($"Error retrieving branch items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);

                return Ok(new {
                    data = new List<ActivityModel>(),
                    recordsTotal = 0 ,
                    recordsFiltered = 0
                });
            }
        }

        [HttpPost("support/activities/allActivities")]
        public async Task<IActionResult> AllActivities([FromBody] TableListRequest request) {

            try{
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
                    Logger.LogActivity($"ACTIVITY DATA - {JsonSerializer.Serialize(activityList)}");
                }

                activityList.Entities ??= new();
                return Ok(new {
                    data = activityList.Entities,
                    recordsTotal = activityList.TotalCount ,
                    recordsFiltered = activityList.TotalCount 
                });
            } catch(Exception ex){
                Logger.LogActivity($"Error retrieving activity logs: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);

                return Ok(new {
                    data = new List<ActivityModel>(),
                    recordsTotal = 0 ,
                    recordsFiltered = 0
                });
            }
            
        }

        [HttpPost("support/departments/allDepartments")]
        public async Task<IActionResult> AllDepartments([FromBody] TableListRequest request) {

            try{
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                        Logger.LogActivity($"DEPARTMENT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.PageSize = 7;

                //..get list of a departments logs
                var departmentdata = await _departmentService.GetDepartmentsAsync(request);

                PagedResponse<DepartmentModel> deparmentList = new ();
                if(departmentdata.HasError){ 
                    Logger.LogActivity($"DEPARTMENT DATA ERROR: Failed to retrieve department items - {JsonSerializer.Serialize(departmentdata)}");
                } else {
                    deparmentList = departmentdata.Data;
                    Logger.LogActivity($"DEPARTMENT DATA - {JsonSerializer.Serialize(deparmentList)}");
                }

                deparmentList.Entities ??= new();
                return Ok(new {
                    data = deparmentList.Entities,
                    recordsTotal = deparmentList.TotalCount ,
                    recordsFiltered = deparmentList.TotalCount 
                });
            } catch(Exception ex){
                Logger.LogActivity($"Error retrieving departments data: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);

                return Ok(new {
                    data = new List<DepartmentModel>(),
                    recordsTotal = 0 ,
                    recordsFiltered = 0
                });
            }
            
        }

        [HttpPost("support/departments/allUnits")]
        public async Task<IActionResult> AllUnits([FromBody] TableListRequest request) {

            try{
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                        Logger.LogActivity($"DEPARTMENT UNITS ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.PageSize = 20;

                //..get list of a departments logs
                var departmentUnitsData = await _departmentUnitService.GetDepartmentUnitsAsync(request);

                PagedResponse<DepartmentUnitModel> deparmentUnitsResult = new ();
                if(departmentUnitsData.HasError){ 
                    Logger.LogActivity($"DEPARTMENT UNITS ERROR: Failed to retrieve department units data - {JsonSerializer.Serialize(departmentUnitsData)}");
                } else {
                    deparmentUnitsResult = departmentUnitsData.Data;
                    Logger.LogActivity($"DEPARTMENT UNITS DATA - {JsonSerializer.Serialize(deparmentUnitsResult)}");
                }

                deparmentUnitsResult.Entities ??= new();
                return Ok(new {
                    data = deparmentUnitsResult.Entities,
                    recordsTotal = deparmentUnitsResult.TotalCount ,
                    recordsFiltered = deparmentUnitsResult.TotalCount 
                });
            } catch(Exception ex){
                Logger.LogActivity($"Error retrieving departments units data: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);

                return Ok(new {
                    data = new List<DepartmentUnitModel>(),
                    recordsTotal = 0 ,
                    recordsFiltered = 0
                });
            }
            
        }

        #endregion

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
