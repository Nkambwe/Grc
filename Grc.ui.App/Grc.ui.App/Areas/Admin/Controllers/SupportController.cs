using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Activity = Grc.ui.App.Enums.Activity;

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
        private readonly IConfigurationFactory _configFactory;
        private readonly ISystemConfiguration _configService;
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
                                 IConfigurationFactory configFactory,
                                 ISystemConfiguration configService,
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
            _configFactory = configFactory;
            _configService = configService;
        }

        [LogActivityResult("User Login", "User logged in to the system", ActivityTypeDefaults.USER_LOGIN, "SystemUser")]
        //[PermissionAuthorization(false, "VIEW_DASHBOARD", "ADMIN_ACCESS")]
        public async Task<IActionResult> Index() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;
                model = await _dDashboardFactory.PrepareAdminDashboardModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
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
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);

                //..prepare deparmenet list model
                model.DepartmentListModel = await _departmentfactory.PrepareDepartmentListModelAsync(currentUser);

            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        #region Dashboard Endpoints

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
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> LockedUsers() {
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
                currentUser.IPAddress = ipAddress;

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
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> UnverifiedUser() {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> DeletedUsers() {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> ActivityRecord()
        {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        #endregion

        #region Access Endpoints

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
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> RoleGroups()
        {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

       
        public async Task<IActionResult> PermissionSets()
        {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> SendEmail()
        {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        [LogActivityResult("User Logout", "User logged out of the system", ActivityTypeDefaults.USER_LOGOUT, "SystemUser")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.Identity?.Name;
                Logger.LogActivity($"Admin user logging out: {username}", "INFO");

                // Update logged_in status in database before signing out
                long id = 0;
                if (!string.IsNullOrEmpty(userId))
                {
                    _ = long.TryParse(userId, out id);
                    await _accessService.UpdateLoggedInStatusAsync(id, false, ipAddress);
                }

                //..sign out from cookie authentication
                await _authService.SignOutAsync(new LogoutModel() { UserId = id, IPAddress = ipAddress });

                // Return JSON response for AJAX
                return Json(new
                {
                    success = true,
                    redirectUrl = Url.Action("Login", "Application", new { area = "" }),
                    message = "Logged out successfully"
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error during admin logout: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "ERROR");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "SUPPORTCONTROLLER-LOGOUT", ex.StackTrace);
                return Json(new
                {
                    success = false,
                    message = LocalizationService.GetLocalizedLabel("Error.Occurance"),
                    error = new { message = "Logout failed. Please try again." }
                });
            }
        }

        #endregion

        #region Role Delegation Endpoints

        public async Task<IActionResult> RoleDelegation() {
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
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        #endregion

        #region Branches

        [HttpPost]
        public async Task<IActionResult> GetBranch(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Branch Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _branchService.GetBranchAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving branch";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var data = result.Data;
                var branch = new {
                    id = data.Id,
                    branchName = data.BranchName,
                    solId = data.SolId,
                    isDeleted = data.IsDeleted,
                    createdOn = data.CreatedOn
                };

                return Ok(new { success = true, data = branch });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving branch record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
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
                        branchName = branch.BranchName,
                        solId = branch.SolId,
                        isDeleted = branch.IsDeleted,
                        createdOn = branch.CreatedOn
                    }).Cast<object>().ToList();
                }

                return Json(new { data = select2Data });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving branches: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> AllBranches([FromBody] TableListRequest request) { 
            
            try{
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                        Logger.LogActivity($"BRANCH DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
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

                var data = (branchList.Entities ??= new()).ToArray();
                var totalPages = branchList.TotalPages <= 0 ? 1 : branchList.TotalPages;
                return Ok(new { data, last_page = totalPages, total_records = branchList.TotalCount });
            } catch(Exception ex){
                Logger.LogActivity($"Error retrieving branch items: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch([FromBody] BranchModel model) {
            try {

                if (model == null) {
                    return Ok(new { success = false, message = "Invalid request data" });
                }

                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _branchService.CreateBranchAsync(model, currentUser.Id, currentUser.IPAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create branch record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Branch created successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating branch record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBranch([FromBody] BranchModel model) {
            try {

                if (model == null) {
                    return Ok(new { success = false, message = "Invalid request data" });
                }

                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                var result = await _branchService.UpdateBranchAsync(model, currentUser.Id, currentUser.IPAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update branch record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Branch update successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updateing branch record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        public async Task<IActionResult> DeleteBranch(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Branch Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = "Delete Branch",
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _branchService.DeleteBranchAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete branchrecord" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting branch record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region Activities
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

        #endregion

        #region Departments

        [HttpPost]
        public async Task<IActionResult> GetDepartment(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Department Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _departmentService.GetDepartmentById(new GrcIdRequest() {
                    RecordId = id,
                    IsDeleted = false,
                    UserId = currentUser.UserId,
                    Action = "Retrieve department",
                    IPAddress = ipAddress,
                    DecryptFields = Array.Empty<string>(),
                    EncryptFields = Array.Empty<string>()
                });

                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving department";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var data = result.Data;
                var department = new {
                    id = data.Id,
                    departmentName = data.DepartmentName,
                    alias = data.DepartmentAlias,
                    code = data.DepartmentCode,
                    isDeleted = data.IsDeleted,
                    departmentHead = data.HeadFullName,
                    departmentHeadEmail = data.HeadEmail,
                    departmentHeadContact = data.HeadContact,
                    departmentHeadDesignation = data.HeadDesignation,
                    units =  data.DepartmentUnits.Select(u => new {
                        id = u.Id,
                        departmentId = u.DepartmentId,
                        unitCode = u.UnitCode,
                        unitName = u.UnitName,
                        department = data.DepartmentName
                    }).ToArray()
                };

                return Ok(new { success = true, data = department });

            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving department record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDepartments() {
            try {

                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DEPARTMENT UNIT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "User record not found" });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                if (currentUser == null){
                    //..session has expired
                   return Ok(new { success = false, message = "User record not found" });
                }
                GrcRequest request = new() {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIEVEDEPARTMENTS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all departments
                var departmentData = await _departmentService.GetDepartmentsAsync(request);

                List<GrcDepartmentRecordResponse> departments;
                if (departmentData.HasError) {
                    departments = new();
                    Logger.LogActivity($"DEPARTMENT DATA ERROR: Failed to retrieve department items - {JsonSerializer.Serialize(departmentData)}");
                } else {
                    departments = departmentData.Data;
                    Logger.LogActivity($"DEPARTMENT DATA - {JsonSerializer.Serialize(departments)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if (departments.Any()) {
                    select2Data = departments.Select(department => new {
                        id = department.Id,
                        text = department.DepartmentName
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving departments: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AllDepartments([FromBody] TableListRequest request) {

            try{
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DEPARTMENT UNIT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "User record not found" });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                if (currentUser == null){
                    //..session has expired
                   return Ok(new { success = false, message = "User record not found" });
                }
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;


                //..get list of a departments logs
                var departmentdata = await _departmentService.GetAllDepartmentsAsync(request);

                PagedResponse<GrcDepartmentRecordResponse> deparmentList = new ();
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

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] DepartmentFullModel model) {
            try {

                if (model == null) {
                    return Ok(new { success = false, message = "Invalid request data" });
                }

                //..get current authenticated user record
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DEPARTMENT UNIT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "User record not found" });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                if (currentUser == null){
                    //..session has expired
                   return Ok(new { success = false, message = "User record not found" });
                }
                var result = await _departmentService.InsertDepartmentAsync(model, currentUser.UserId, currentUser.IPAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create department record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Department created successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating department record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepartment([FromBody] DepartmentFullModel model) {
            try {

                if (model == null) {
                    return Ok(new { success = false, message = "Invalid request data" });
                }
                
                //..get current authenticated user record
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DEPARTMENT UNIT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "User record not found" });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                if (currentUser == null){
                    //..session has expired
                   return Ok(new { success = false, message = "User record not found" });
                }
                var result = await _departmentService.UpdateDepartmentAsync(model, currentUser.UserId, currentUser.IPAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update department record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Department update successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updateing department record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Department Id is required" });

                var currentUser = userResponse.Data;
                var result = await _departmentService.DeleteDepartmentAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete department" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting department record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region Department Units

        [HttpPost]
        public async Task<IActionResult> GetUnit(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Department unit Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;

                var result = await _departmentUnitService.GetUnitById(new GrcIdRequest() {
                    RecordId = id,
                    IsDeleted = false,
                    UserId = currentUser.UserId,
                    Action = "Retrieve department unit",
                    IPAddress = ipAddress,
                    DecryptFields = Array.Empty<string>(),
                    EncryptFields = Array.Empty<string>()
                });

                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving department unit";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var data = result.Data;
                var unit = new {
                    id = data.Id,
                    departmentId = data.DepartmentId,
                    unitCode = data.UnitCode,
                    unitName = data.UnitName,
                    department = data.Department,
                    unitHead = data.UnitHead,
                    unitContactEmail = data.UnitContactEmail,
                    unitContactNumber = data.UnitContactNumber,
                    unitHeadDesignation = data.UnitHeadDesignation
                };

                return Ok(new { success = true, data = unit });

            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving department record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnitsMiniList(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = Array.Empty<object>() });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "User Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetSelectedUnitsAsync(currentUser.UserId, id, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving department units";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = Array.Empty<object>() });
                }

                var unitData = result.Data;
                var listData = unitData.Select(unit => new {
                    code = unit.UnitCode,
                    unitName = unit.UnitName
                }).ToList();

                return Ok(new { success = true, data = listData });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { success = false, data = Array.Empty<object>() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnits() { 
            try {

                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                        Logger.LogActivity($"UNITS LIST ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }

                GrcRequest request = new() {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIEVEUNITS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all units
                var  unitsData = await _departmentUnitService.GetUnitsAsync(request);

                List<GrcDepartmentUnitResponse> units;
                if(unitsData.HasError){ 
                    units = new ();
                    Logger.LogActivity($"UNITS DATA ERROR: Failed to retrieve unit items - {JsonSerializer.Serialize(unitsData)}");
                } else {
                    units = unitsData.Data;
                    Logger.LogActivity($"UNITS DATA - {JsonSerializer.Serialize(units)}");
                }

                //..get ajax data
                List<object> select2Data = new();
                if(units.Any()){ 
                    select2Data = units.Select(unit => new {                        
                        id = unit.Id,
                        text = unit.UnitName 
                    }).Cast<object>().ToList();
                }

                return Json(new { results = select2Data });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving units: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
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
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.PageSize = 20;

                //..get list of a departments logs
                var departmentUnitsData = await _departmentUnitService.GetDepartmentUnitsAsync(request);

                PagedResponse<GrcDepartmentUnitResponse> deparmentUnitsResult = new ();
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
        
        [HttpPost]
        public async Task<IActionResult> CreateUnit([FromBody] DepartmentFullUnitModel model) {
            try {

                if (model == null) {
                    return Ok(new { success = false, message = "Invalid request data" });
                }

                
                //..get current authenticated user record
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DEPARTMENT UNIT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "User record not found" });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                if (currentUser == null){
                    //..session has expired
                   return Ok(new { success = false, message = "User record not found" });
                }
                var result = await _departmentUnitService.InsertDepartmentUnitAsync(model, currentUser.UserId, currentUser.IPAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create department unit record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Department unit created successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating department unit record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUnit([FromBody] DepartmentFullUnitModel model) {
            try {

                if (model == null) {
                    return Ok(new { success = false, message = "Invalid request data" });
                }

               var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    Logger.LogActivity($"DEPARTMENT UNIT DATA ERROR: Failed to Current user record - {JsonSerializer.Serialize(grcResponse)}");
                    return Ok(new { success = false, message = "User record not found" });
                }

                //..update with user data
                var currentUser = grcResponse.Data;
                if (currentUser == null){
                    //..session has expired
                   return Ok(new { success = false, message = "User record not found" });
                }
                var result = await _departmentUnitService.UpdateDepartmentUnitAsync(model, currentUser.UserId, currentUser.IPAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update department unit record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Department unit updated successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updateing department unit record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUnit(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Department unit Id is required" });

                var currentUser = userResponse.Data;
                var result = await _departmentUnitService.DeleteDepartmentUnitAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete department unit" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error deleting department record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region Configuration

        public async Task<IActionResult> PasswordPolicy() {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> ApplicationSetting() {
            var model = new ConfigurationModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;
                model = await _configFactory.PrepareConfigurationModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        #endregion

        #region System Activity

        [LogActivityResult("System Activity Retrieve", "User retrieved system activity", ActivityTypeDefaults.ACTIVITY_RETRIEVED, "SystemActivity")]
        public async Task<IActionResult> GetSystemActivity(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "System Activity Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetSystemActivityIdAsync(currentUser.UserId, id, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving system Activity";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var activity = result.Data;
                var activityRecord = new {
                    id = activity.Id,
                    action = activity.Action,
                    userId = activity.Id,
                    entityName = activity.EntityName,
                    activityType = activity.ActivityType,
                    accessedBy = activity.AccessedBy,
                    ipAddress = activity.IpAddress,
                    activityDate = activity.ActivityDate
                };

                return Ok(new { success = true, data = activityRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system activity: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        public async Task<IActionResult> GetPagedSystemActivities([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("SYSTEM ACTIVITY DATA ERROR: Failed to get system activities");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIEVEALLSYSTEMACTIVITIES.GetDescription();

                var result = await _accessService.GetPagedActivitiesAsync(request);
                PagedResponse<GrcSystemActivityResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcSystemActivityResponse>())
                    .Select(activity => new {
                        id = activity.Id,
                        action = activity.Action,
                        userId = activity.Id,
                        entityName = activity.EntityName,
                        activityType = activity.ActivityType,
                        accessedBy = activity.AccessedBy,
                        ipAddress = activity.IpAddress,
                        activityDate = activity.ActivityDate
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system activities: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region System Users

        [PermissionAuthorization(true, "CANVIEWUSER")]
        public async Task<IActionResult> Users() {
            var model = new AdminDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [LogActivityResult("Retrieve User", "User retrieved user record", ActivityTypeDefaults.USER_RETRIEVED, "SystemUser")]
        [PermissionAuthorization(true, "CANVIEWUSER")]
        public async Task<IActionResult> GetUser(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "User Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetUserByIdAsync(currentUser.UserId, id, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving user record";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var user = result.Data;
                var userRecord = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    middleName = user.MiddleName,
                    userName = user.UserName,
                    fullName = !string.IsNullOrWhiteSpace(user.MiddleName) ?
                    $"{user.FirstName} {user.MiddleName.Trim()} {user.LastName}": 
                    $"{user.FirstName} {user.LastName}",
                    emailAddress = user.Email,
                    displayName = user.DisplayName,
                    phoneNumber = user.PhoneNumber,
                    pfNumber = user.PFNumber,
                    solId = user.SolId,
                    roleId = user.RoleId,
                    roleName = user.RoleName,
                    roleGroup = user.RoleGroup,
                    departmentId = user.DepartmentId,
                    department = user.DepartmentName,
                    unitCode = user.UnitCode,
                    isActive = user.IsActive,
                    isVerified = user.IsVerified,
                    createdOn = user.CreatedOn,
                    createdBy = user.CreatedBy,
                    modifiedOn = user.ModifiedOn,
                    modifiedBy = user.ModifiedBy
                };

                return Ok(new { success = true, data = userRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWUSER")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {

                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"USER LIST ERROR: Failed to retrieve Current user records - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIVEUSERS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = new string[] { "FirstName","MiddleName","LastName", "PhoneNumber", "PFNumber", "Email" }
                };

                //..get list of all users
                var usersData = await _accessService.GetUsersAsync(request);

                List<UserResponse> users;
                if (usersData.HasError)
                {
                    users = new();
                    Logger.LogActivity($"USERS DATA ERROR: Failed to retrieve system user records - {JsonSerializer.Serialize(usersData)}");
                }
                else
                {
                    users = usersData.Data.Data;
                    Logger.LogActivity($"USERS DATA - {JsonSerializer.Serialize(users)}");
                }

                //..get ajax data
                List<object> listData = new();
                if (users.Any())
                {
                    listData = users.Select(user => new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        middleName = user.MiddleName,
                        userName = user.UserName,
                        emailAddress = user.Email,
                        displayName = user.DisplayName,
                        phoneNumber = user.PhoneNumber,
                        pfNumber = user.PFNumber,
                        solId = user.SolId,
                        roleId = user.RoleId,
                        roleName = user.RoleName,
                        roleGroup = user.RoleGroup,
                        departmentId = user.DepartmentId,
                        department = user.DepartmentName,
                        unitCode = user.UnitCode,
                        isActive = user.IsActive,
                        isVerified = user.IsVerified,
                        createdOn = user.CreatedOn,
                        createdBy = user.CreatedBy,
                        modifiedOn = user.ModifiedOn,
                        modifiedBy = user.ModifiedBy
                    }).Cast<object>().ToList();
                }

                return Json(new { data = listData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving users: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWUSER")]
        public async Task<IActionResult> GetPagedUsers([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.USER_RETRIEVED.GetDescription();

                var result = await _accessService.GetPagedUsersAsync(request);
                PagedResponse<UserResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<UserResponse>())
                    .Select(user => new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        middleName = user.MiddleName,
                        userName = user.UserName,
                        emailAddress = user.Email,
                        fullName = !string.IsNullOrWhiteSpace(user.MiddleName) ?
                        $"{user.FirstName} {user.MiddleName.Trim()} {user.LastName}": 
                        $"{user.FirstName} {user.LastName}",
                        displayName = user.DisplayName,
                        phoneNumber = user.PhoneNumber,
                        pfNumber = user.PFNumber,
                        solId = user.SolId,
                        roleId = user.RoleId,
                        roleName = user.RoleName,
                        roleGroup = user.RoleGroup,
                        departmentId = user.DepartmentId,
                        departmentName = user.DepartmentName,
                        unitCode = user.UnitCode,
                        isActive = user.IsActive,
                        isVerified = user.IsVerified,
                        createdOn = user.CreatedOn,
                        createdBy = user.CreatedBy,
                        modifiedOn = user.ModifiedOn,
                        modifiedBy = user.ModifiedBy
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving system users: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANAPPROVEUSER")]
        public async Task<IActionResult> GetUnapprovedUsers([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.UNAPPROVED_ACCOUNTS.GetDescription();

                var result = await _accessService.GetUnapprovedUsersAsync(request);
                PagedResponse<UserResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<UserResponse>())
                    .Select(user => new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        middleName = user.MiddleName,
                        userName = user.UserName,
                        emailAddress = user.Email,
                        displayName = $"{user.FirstName} {user.LastName}",
                        phoneNumber = user.PhoneNumber,
                        pfNumber = user.PFNumber,
                        solId = user.SolId,
                        roleId = user.RoleId,
                        roleName = user.RoleName,
                        roleGroup = user.RoleGroup,
                        departmentId = user.DepartmentId,
                        departmentName = user.DepartmentName,
                        unitCode = user.UnitCode,
                        isActive = user.IsActive,
                        isVerified = user.IsVerified,
                        createdOn = user.CreatedOn.ToString("yyyy-MM-dd"),
                        createdBy = user.CreatedBy ?? string.Empty,
                        modifiedOn = user.ModifiedOn.HasValue ? user.ModifiedOn.Value.ToString("yyyy-MM-dd") : string.Empty,
                        modifiedBy = user.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system users: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANLOCKUSERACCOUNT")]
        public async Task<IActionResult> GetLockedUsers([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.USER_LOCKED_ACCOUNTS.GetDescription();

                var result = await _accessService.GetLockedUsersAsync(request);
                PagedResponse<UserResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<UserResponse>())
                    .Select(user => new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        middleName = user.MiddleName,
                        userName = user.UserName,
                        emailAddress = user.Email,
                        displayName = $"{user.FirstName} {user.LastName}",
                        phoneNumber = user.PhoneNumber,
                        pfNumber = user.PFNumber,
                        solId = user.SolId,
                        roleId = user.RoleId,
                        roleName = user.RoleName,
                        roleGroup = user.RoleGroup,
                        departmentId = user.DepartmentId,
                        departmentName = user.DepartmentName,
                        unitCode = user.UnitCode,
                        isActive = user.IsActive,
                        isVerified = user.IsVerified,
                        createdOn = user.CreatedOn.ToString("yyyy-MM-dd"),
                        createdBy = user.CreatedBy ?? string.Empty,
                        modifiedOn = user.ModifiedOn.HasValue ? user.ModifiedOn.Value.ToString("yyyy-MM-dd"): string.Empty,
                        modifiedBy = user.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving locked user accounts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANVERIFYUSERACCOUNT")]
        public async Task<IActionResult> GetUnverifiedUsers([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.UNVERIFIED_ACCOUNTS.GetDescription();

                var result = await _accessService.GetUnverifiedUsersAsync(request);
                PagedResponse<UserResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<UserResponse>())
                    .Select(user => new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        middleName = user.MiddleName,
                        userName = user.UserName,
                        emailAddress = user.Email,
                        displayName = $"{user.FirstName} {user.LastName}",
                        phoneNumber = user.PhoneNumber,
                        pfNumber = user.PFNumber,
                        solId = user.SolId,
                        roleId = user.RoleId,
                        roleName = user.RoleName,
                        roleGroup = user.RoleGroup,
                        departmentId = user.DepartmentId,
                        departmentName = user.DepartmentName,
                        unitCode = user.UnitCode,
                        isActive = user.IsActive,
                        isVerified = user.IsVerified,
                        createdOn = user.CreatedOn.ToString("yyyy-MM-dd"),
                        createdBy = user.CreatedBy ?? string.Empty,
                        modifiedOn = user.ModifiedOn.HasValue ? user.ModifiedOn.Value.ToString("yyyy-MM-dd") : string.Empty,
                        modifiedBy = user.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving locked user accounts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANDELETEUSER")]
        public async Task<IActionResult> GetDeletedAccounts([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.DELETED_ACCOUNTS.GetDescription();

                var result = await _accessService.GetDeletedUsersAsync(request);
                PagedResponse<UserResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<UserResponse>())
                    .Select(user => new {
                        id = user.Id,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        middleName = user.MiddleName,
                        userName = user.UserName,
                        emailAddress = user.Email,
                        displayName = $"{user.FirstName} {user.LastName}",
                        phoneNumber = user.PhoneNumber,
                        pfNumber = user.PFNumber,
                        solId = user.SolId,
                        roleId = user.RoleId,
                        roleName = user.RoleName,
                        roleGroup = user.RoleGroup,
                        departmentId = user.DepartmentId,
                        departmentName = user.DepartmentName,
                        unitCode = user.UnitCode,
                        isActive = user.IsActive,
                        isVerified = user.IsVerified,
                        createdOn = user.CreatedOn.ToString("yyyy-MM-dd"),
                        createdBy = user.CreatedBy ?? string.Empty,
                        modifiedOn = user.ModifiedOn.HasValue ? user.ModifiedOn.Value.ToString("yyyy-MM-dd") : string.Empty,
                        modifiedBy = user.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving locked user accounts: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("User Added", "User added user record", ActivityTypeDefaults.USER_ADDED, "SystemUser")]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANADDUSER")]
        public async Task<IActionResult> CreateUser([FromBody] UserViewModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new
                    {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid user data" });
                }

                var result = await _accessService.CreateUserAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create user record" });

                var user = result.Data;
                return Ok(new
                {
                    success = user.Status,
                    message = user.Message,
                    data = new
                    {
                        status = user.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("User Edited", "User updated user record", ActivityTypeDefaults.USER_EDITED, "SystemUser")]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER")]
        public async Task<IActionResult> UpdateUser([FromBody] UserViewModel request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid user data" });
                }

                var result = await _accessService.UpdateUserAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update user record" });

                var data = result.Data;
                return Ok(new
                {
                    success = data.Status,
                    message = data.Message,
                    data = new
                    {
                        status = data.Status,
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error update user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("User Deleted", "User deleted User record", ActivityTypeDefaults.USER_DELETED, "SystemUser")]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANDELETEUSER")]
        public async Task<IActionResult> DeleteUser(long id) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "User Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.USER_DELETED.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.DeleteUserAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete user record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANLOCKUSERACCOUNT")]
        public async Task<IActionResult> RestoreUserAccount(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "User Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.RESTORE_ACCOUNT.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };


                var result = await _accessService.RestoreUserAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to lock user record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error locking user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANAPPROVEUSER")]
        public async Task<IActionResult> ApproveUser(long id) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (id == 0) {
                    return Ok(new { success = false, message = "Invalid user request" });
                }

                var request = new ApproveUserViewModel(){
                    Id = id,
                    IsApproved = true,
                    IsVerified = true,
                };


                var same = true;
                if (same) {
                    return Ok(new { success = false, message = "You cannot approve a user created or verified by you" });
                }

                var result = await _accessService.ApproveUserAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update user record" });

                var data = result.Data;
                return Ok(new{ success = data.Status, message = data.Message,data = new {status = data.Status,}});
            } catch (Exception ex) {
                Logger.LogActivity($"Error update user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANVERIFYUSERACCOUNT")]
        public async Task<IActionResult> VerifyUser(long id) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (id == 0) {
                    return Ok(new { success = false, message = "Invalid user request" });
                }

                var request = new ApproveUserViewModel(){
                    Id = id,
                    IsApproved = true,
                    IsVerified = true,
                };

                var same = true;
                if (same) {
                    return Ok(new { success = false, message = "You cannot verify a user created by you" });
                }

                var result = await _accessService.VerifyUserAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update user record" });

                var data = result.Data;
                return Ok(new{ success = data.Status, message = data.Message,data = new {status = data.Status,}});
            } catch (Exception ex) {
                Logger.LogActivity($"Error update user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }
        
        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANLOCKUSERACCOUNT")]
        public async Task<IActionResult> LockUserAccount(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "User Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.LOCK_ACCOUNT.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.LockUserAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to lock user record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error locking user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANLOCKUSERACCOUNT")]
        public async Task<IActionResult> UnLockUserAccount(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "User Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.UNLOCK_ACCOUNT.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.UnlockUserAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to unlock user record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error unocking user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANMODIFYUSER", "CANRESETUSERPASSWORDS")]
        public async Task<IActionResult> PasswordReset(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "User Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.LOCK_ACCOUNT.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.PasswordResetAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to lock user record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            } catch (Exception ex) {
                Logger.LogActivity($"Error locking user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [PermissionAuthorization(true, "CANVIEWUSER", "CANDOWNLOADUSERREPORT")]
        public async Task<IActionResult> ExportUserList() {

            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                EncryptFields = Array.Empty<string>(),  
                DecryptFields = Array.Empty<string>(),
                Action = Activity.USER_LIST.GetDescription()
            };

            var result = await _accessService.GetUsersAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve user data" });

            var data = result.Data.Data ?? new List<UserResponse>();
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("GRC Active users");

            //..headers
            string[] headers = {
                "FULL NAME",
                "PF NO.",
                "EMAIL",
                "DEPARTMENT",
                "USERNAME",
                "ROLE",
                "ACTIVE",
                "VERIFIED",
                "APPROVED",
                "LAST LOGIN"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            //..header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            //..header height
            ws.Row(1).Height = 30;

            //..add data
            int row = 2;
            foreach (var p in data) {
                ws.Cell(row, 1).Value = !string.IsNullOrWhiteSpace(p.MiddleName)? 
                    $"{p.FirstName.Trim()} {p.MiddleName.Trim()} {p.LastName.Trim()}":
                    $"{p.FirstName.Trim()} {p.LastName.Trim()}";
                ws.Cell(row, 2).Value = (p.PFNumber ?? string.Empty).Trim();
                ws.Cell(row, 3).Value = (p.Email ?? string.Empty).Trim();
                ws.Cell(row, 4).Value = (p.DepartmentName ?? string.Empty).Trim();
                ws.Cell(row, 5).Value = (p.UserName ?? string.Empty).Trim();
                ws.Cell(row, 6).Value = (p.RoleName ?? string.Empty).Trim();
                ws.Cell(row, 7).Value = p.IsActive ? "YES" : "NO";
                ws.Cell(row, 8).Value = p.IsVerified ? "YES" : "NO";
                ws.Cell(row, 9).Value = p.IsApproved ? "YES" : "NO";
                SetSafeDate(ws.Cell(row, 10), p.LastLogindate);
                row++;
            }

            int lastDataRow = row - 1;

            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.LightGray;
            ws.Range(2, 4, lastDataRow, 4).Style.Fill.BackgroundColor = XLColor.Gray;
            ws.Range(1, 4, lastDataRow, 4).Style.Border.LeftBorder = XLBorderStyleValues.Thin;

            //..add header filtersfilters
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

            //..freeze header
            ws.SheetView.FreezeRows(1);
            ws.Column(10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            //..auto-fit columns to content
            ws.Columns().AdjustToContents(10, 60);
            ws.Column(1).Width = 70;
            ws.Column(2).Width = 10;
            ws.Column(5).Width = 20;
            ws.Column(7).Width = 10;
            ws.Column(8).Width = 10;
            ws.Column(9).Width = 10;
            ws.Column(10).Width = 15; 

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"GRCUSERS-{DateTime.Today:yyyy-MM}.xlsx");
        }

        #endregion

        #region System Role

        [LogActivityResult("Retrieve Role", "User retrieved role record", ActivityTypeDefaults.ROLE_RETRIEVED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRole(long id) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Role Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetRoleByIdAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var role = result.Data;
                var roleRecord = new {
                    id = role.Id,
                    roleName = role.RoleName,
                    roleDescription = role.Description,
                    groupId = role.GroupId,
                    groupName = role.GroupName,
                    isDeleted = role.IsDeleted,
                    createdOn = role.CreatedOn,
                    createdBy = role.CreatedBy,
                    modifiedOn = role.ModifiedOn,
                    modifiedBy = role.ModifiedBy
                };

                return Ok(new { success = true, data = roleRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        public async Task<IActionResult> GetRoleGroupsMiniList(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = Array.Empty<object>() });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "User Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetSelectedRoleGroupsAsync(currentUser.UserId, id, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role groups";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = Array.Empty<object>() });
                }

                var roleData = result.Data;
                var listData = roleData.Select(roleGroup => new {
                    id = roleGroup.Id,
                    groupName = roleGroup.GroupName
                }).ToList();

                return Ok(new { success = true, data = listData });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving user record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { success = false, data = Array.Empty<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoles() {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"ROLE LIST ERROR: Failed to retrieve system role records - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIVEROLES.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all roles
                var rolesData = await _accessService.GetRolesAsync(request);

                List<GrcRoleResponse> roles;
                if (rolesData.HasError)
                {
                    roles = new();
                    Logger.LogActivity($"ROLES DATA ERROR: Failed to retrieve system role records - {JsonSerializer.Serialize(rolesData)}");
                }
                else
                {
                    roles = rolesData.Data.Data;
                    Logger.LogActivity($"ROLE DATA - {JsonSerializer.Serialize(roles)}");
                }

                //..get ajax data
                List<object> listData = new();
                if (roles.Any())
                {
                    listData = roles.Select(role => new {
                        id = role.Id,
                        roleName = role.RoleName,
                        roleDescription = role.Description,
                        groupName = role.GroupName,
                        isDeleted = role.IsDeleted,
                        createdOn = role.CreatedOn,
                        createdBy = role.CreatedBy,
                        modifiedOn = role.ModifiedOn,
                        modifiedBy = role.ModifiedBy
                    }).Cast<object>().ToList();
                }

                return Json(new { data = listData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving roles: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPagedRoles([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("ROLE DATA ERROR: Failed to get role record");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIVEROLES.GetDescription();

                var result = await _accessService.GetPagedRolesAsync(request);
                PagedResponse<GrcRoleResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcRoleResponse>())
                    .Select(role => new {
                        id = role.Id,
                        roleName = role.RoleName,
                        roleDescription = role.Description,
                        groupName = role.GroupName,
                        groupId = role.GroupId,
                        isDeleted = role.IsDeleted,
                        isApproved = role.IsApproved,
                        isVerified = role.IsVerified,
                        createdOn = role.CreatedOn,
                        createdBy = role.CreatedBy,
                        modifiedOn = role.ModifiedOn,
                        modifiedBy = role.ModifiedBy
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system roles: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Added", "User added system role record", ActivityTypeDefaults.ROLE_ADDED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANADDROLE")]
        public async Task<IActionResult> CreateRole([FromBody] RoleViewModel request) {
            try {
                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null) {
                    return Ok(new { success = false, message = "Invalid role data" });
                }

                var result = await _accessService.CreateRoleAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create role record" });

                var role = result.Data;
                return Ok(new
                {
                    success = role.Status,
                    message = role.Message,
                    data = new
                    {
                        status = role.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Edited", "User updated system role", ActivityTypeDefaults.ROLE_EDITED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANMODIFYROLE")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid user data" });
                }

                var result = await _accessService.UpdateRoleAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update system role record" });

                var data = result.Data;
                return Ok(new
                {
                    success = data.Status,
                    message = data.Message,
                    data = new
                    {
                        status = data.Status,
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error update system role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Deleted", "User deleted System Role record", ActivityTypeDefaults.ROLE_DELETED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANMODIFYROLE","CANDELETEROLE")]
        public async Task<IActionResult> DeleteRole(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Role Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.ROLE_DELETED.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.DeleteRoleAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete system role record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting system role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region System Role Permissions
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> RolePermissions() {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);

            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [LogActivityResult("Role Permissions", "User retrieved role record with related permissions", ActivityTypeDefaults.ROLE_PERMISSIONS_RETRIEVED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoleWithPermissions(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Role Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetRoleWithPermissionsAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var role = result.Data;
                var roleRecord = new {
                    id = role.Id,
                    roleName = role.RoleName,
                    roleDescription = role.Description,
                    groupName = role.GroupName,
                    isDeleted = role.IsDeleted,
                    createdOn = role.CreatedOn,
                    createdBy = role.CreatedBy,
                    modifiedOn = role.ModifiedOn,
                    modifiedBy = role.ModifiedBy,
                    sets = role.PermissionSets != null ? role.PermissionSets.Select(set => new {
                        id = set.Id,
                        setName = set.SetName,
                        setDescription = set.SetDescription,
                        isAssigned = set.IsAssigned
                    }).ToArray() : Array.Empty<object>(),
                    permissions = role.Permissions != null ? role.Permissions.Select(permission => new {
                        id = permission.Id,
                        setId = permission.SetId,
                        permissionName = permission.PermissionName,
                        permissionDescription = permission.PermissionDescription,
                        isAssigned = permission.IsAssigned
                    }).ToArray():
                    Array.Empty<object>()
                };

                return Ok(new { success = true, data = roleRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [LogActivityResult("Role Permission sets", "User retrieved role record with related permission set", ActivityTypeDefaults.ROLE_PERMISSION_SETS_RETRIEVED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoleWithPermissionSets(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Role Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetRoleWithPermissionSetsAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var role = result.Data;
                var roleRecord = new
                {
                    id = role.Id,
                    roleName = role.RoleName,
                    roleDescription = role.Description,
                    groupName = role.GroupName,
                    isDeleted = role.IsDeleted,
                    createdOn = role.CreatedOn,
                    createdBy = role.CreatedBy,
                    modifiedOn = role.ModifiedOn,
                    modifiedBy = role.ModifiedBy,
                    permissionSets = role.PermissionSets.Select(set => new {
                        id = set.Id,
                        setName = set.SetName,
                        setDescription = set.SetDescription,
                        isAssigned = set.IsAssigned
                    }).ToList()
                };

                return Ok(new { success = true, data = roleRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [LogActivityResult("Role Users", "User retrieved role record with related users", ActivityTypeDefaults.ROLE_USERS_RETRIEVED, "SystemRole")]
        [PermissionAuthorization(true, "CANVIEWROLES", "CANVIEWUSER")]
        public async Task<IActionResult> GetRoleWithUsers(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Role Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetRoleWithUsersAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var role = result.Data;
                var roleRecord = new
                {
                    id = role.Id,
                    roleName = role.RoleName,
                    roleDescription = role.Description,
                    groupName = role.GroupName,
                    isDeleted = role.IsDeleted,
                    createdOn = role.CreatedOn,
                    createdBy = role.CreatedBy,
                    modifiedOn = role.ModifiedOn,
                    modifiedBy = role.ModifiedBy,
                    users = role.Users.Select(user => new {
                        id = user.Id,
                        userName = !string.IsNullOrWhiteSpace(user.MiddleName) ? $"{user.FirstName} {user.MiddleName} {user.LastName}": $"{user.FirstName} {user.LastName}",
                        emailAddress = user.Email,
                        pfNumber = user.PFNumber,
                        userStatus = user.IsActive,
                        userDepartment = user.DepartmentName,
                        userUnit = user.UnitCode
                    }).ToList()
                };

                return Ok(new { success = true, data = roleRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region System Role Groups

        [LogActivityResult("Role Group", "User retrieved role group", ActivityTypeDefaults.ROLE_GROUP_RETRIEVED, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoleGroupWithRoles(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Role Group Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetRoleGroupWithRolesByIdAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role group";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var roleGroup = result.Data;
                var userRecord = new
                {
                    id = roleGroup.Id,
                    groupName = roleGroup.GroupName,
                    groupDescription = roleGroup.Description,
                    groupScope = roleGroup.GroupScope,
                    groupCategory = roleGroup.GroupCategory,
                    groupType = roleGroup.GroupType,
                    department = roleGroup.DepartmentName,
                    isDeleted = roleGroup.IsDeleted,
                    isVerified = roleGroup.IsVerified,
                    isApproved = roleGroup.IsApproved,
                    createdOn = roleGroup.CreatedOn,
                    createdBy = roleGroup.CreatedBy,
                    modifiedOn = roleGroup.ModifiedOn,
                    modifiedBy = roleGroup.ModifiedBy,
                    roles = roleGroup.Roles
                };

                return Ok(new { success = true, data = userRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoleGroupLists() {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();
                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"ROLE LIST ERROR: Failed to retrieve current user record - {JsonSerializer.Serialize(grcResponse)}");
                }
                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIVEROLEGROUPS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };
                //..get list of all role groups
                var rolesData = await _accessService.GetRoleGroupsAsync(request);
                List<GrcRoleGroupResponse> roleGroups;
                if (rolesData.HasError)
                {
                    roleGroups = new();
                    Logger.LogActivity($"ROLES DATA ERROR: Failed to retrieve system role groups - {JsonSerializer.Serialize(rolesData)}");
                }
                else
                {
                    roleGroups = rolesData.Data.Data;
                    Logger.LogActivity($"ROLE GROUP DATA - {JsonSerializer.Serialize(roleGroups)}");
                }

                //..get ajax data - removed Cast<object>()
                var listData = roleGroups.Select(roleGroup => new {
                    id = roleGroup.Id,
                    groupName = roleGroup.GroupName
                }).ToList();

                Logger.LogActivity($"ROLE GROUP DATA2 - {JsonSerializer.Serialize(listData)}");
                return Json(new { data = listData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving roles: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { data = new List<object>() }); // Changed results to data for consistency
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoleGroups() {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"ROLE LIST ERROR: Failed to retrieve current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.RETRIVEROLEGROUPS.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all role groups
                var rolesData = await _accessService.GetRoleGroupsAsync(request);

                List<GrcRoleGroupResponse> roleGroups;
                if (rolesData.HasError)
                {
                    roleGroups = new();
                    Logger.LogActivity($"ROLES DATA ERROR: Failed to retrieve system role groups - {JsonSerializer.Serialize(rolesData)}");
                }
                else
                {
                    roleGroups = rolesData.Data.Data;
                    Logger.LogActivity($"ROLE GROUP DATA - {JsonSerializer.Serialize(roleGroups)}");
                }

                //..get ajax data
                List<object> listData = new();
                if (roleGroups.Any())
                {
                    listData = roleGroups.Select(roleGroup => new {
                        id = roleGroup.Id,
                        groupName = roleGroup.GroupName,
                        groupDescription = roleGroup.Description,
                        department = roleGroup.DepartmentName,
                        groupScope = roleGroup.GroupScope,
                        groupCategory = roleGroup.GroupCategory,
                        groupType = roleGroup.GroupType,
                        isDeleted = roleGroup.IsDeleted,
                        isVerified = roleGroup.IsVerified,
                        isApproved = roleGroup.IsApproved,
                        createdOn = roleGroup.CreatedOn,
                        createdBy = roleGroup.CreatedBy,
                        modifiedOn = roleGroup.ModifiedOn,
                        modifiedBy = roleGroup.ModifiedBy
                    }).Cast<object>().ToList();
                }

                return Json(new { data = listData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving roles: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPagedRoleGroups([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("ROLE GROUP DATA ERROR: Failed to get role group");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIVEROLEGROUPS.GetDescription();

                var result = await _accessService.GetPagedRoleGroupsAsync(request);
                PagedResponse<GrcRoleGroupResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcRoleGroupResponse>())
                .Select(roleGroup => new {
                    id = roleGroup.Id,
                    groupName = roleGroup.GroupName,
                    groupDescription = roleGroup.Description,
                    department = roleGroup.DepartmentName,
                    groupScope = roleGroup.GroupScope,
                    groupCategory = roleGroup.GroupCategory,
                    groupType = roleGroup.GroupType,
                    isDeleted = roleGroup.IsDeleted,
                    isVerified = roleGroup.IsVerified,
                    isApproved = roleGroup.IsApproved,
                    createdOn = roleGroup.CreatedOn,
                    createdBy = roleGroup.CreatedBy,
                    modifiedOn = roleGroup.ModifiedOn,
                    modifiedBy = roleGroup.ModifiedBy,
                    roles = roleGroup.Roles?.Select(role => new {
                        id = role.Id,
                        roleName = role.RoleName,
                        description = role.Description,
                        groupId = role.GroupId,
                        groupName = role.GroupName,
                        groupCategory = role.GroupCategory,
                        deleted = role.IsDeleted,
                        verified = role.IsVerified,
                        approved = role.IsApproved,
                        createdBy = role.CreatedBy,
                        createdOn = role.CreatedOn,
                        modifiedBy = role.ModifiedBy,
                        modifiedOn = role.ModifiedOn
                    }).ToList()
                }).ToList();

                //..handle null roles after projection
                foreach (var entity in pagedEntities) {
                    if (entity.roles == null)
                    {
                        var anonymousType = entity.GetType();
                        var rolesProperty = anonymousType.GetProperty("roles");
                        rolesProperty?.SetValue(entity, new List<object>());
                    }
                }

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system role groups: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Group Added", "User added system role group", ActivityTypeDefaults.ROLE_GROUP_ADDED, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANADDROLE")]
        public async Task<IActionResult> CreateRoleGroup([FromBody] RoleGroupViewModel request) {
            try {
                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new
                    {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid role group data" });
                }

                var result = await _accessService.CreateRoleGroupAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create role group" });

                var roleGroup = result.Data;
                return Ok(new
                {
                    success = roleGroup.Status,
                    message = roleGroup.Message,
                    data = new
                    {
                        status = roleGroup.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Group Edited", "User updated system role group", ActivityTypeDefaults.ROLE_GROUP_EDITED, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANMODIFYROLE")]
        public async Task<IActionResult> UpdateRoleGroup([FromBody] RoleGroupViewModel request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid user data" });
                }

                var result = await _accessService.UpdateRoleGroupAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update system role group" });

                var data = result.Data;
                return Ok(new
                {
                    success = data.Status,
                    message = data.Message,
                    data = new
                    {
                        status = data.Status,
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error update system role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Group Deleted", "User deleted System Role Group", ActivityTypeDefaults.ROLE_GROUP_DELETED, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANDELETEROLE")]
        public async Task<IActionResult> DeleteRoleGroup(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Role Group Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.ROLE_GROUP_DELETED.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.DeleteRoleGroupAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete system role group" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting role group record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region System Role Group permission sets

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> RoleGroupPermissions()
        {
            var model = new AdminDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "SUPPORT-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [LogActivityResult("Role Group Retrieved", "User retrieved role group", ActivityTypeDefaults.ROLE_GROUP_RETRIEVED, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetRoleGroupWithPermissions(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0)
                {
                    return BadRequest(new { success = false, message = "Role Group Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetRoleGroupWithPermissionSetsByIdAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving role group";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var roleGroup = result.Data;
                var groupData = new
                {
                    id = roleGroup.Id,
                    groupName = roleGroup.GroupName,
                    groupDescription = roleGroup.Description,
                    groupScope = roleGroup.GroupScope,
                    groupCategory = roleGroup.GroupCategory,
                    groupType = roleGroup.GroupType,
                    department = roleGroup.DepartmentName,
                    isDeleted = roleGroup.IsDeleted,
                    isVerified = roleGroup.IsVerified,
                    isApproved = roleGroup.IsApproved,
                    createdOn = roleGroup.CreatedOn,
                    createdBy = roleGroup.CreatedBy,
                    modifiedOn = roleGroup.ModifiedOn,
                    modifiedBy = roleGroup.ModifiedBy,
                    permissionSets = roleGroup.PermissionSets
                };

                return Ok(new { success = true, data = groupData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPagedRoleGroupWithPermissionSets([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("ROLE GROUP DATA ERROR: Failed to get role group");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.RETRIVEROLEGROUPS.GetDescription();

                var result = await _accessService.GetPagedRoleGroupWithPermissionSetsAsync(request);
                PagedResponse<GrcRoleGroupResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcRoleGroupResponse>())
                .Select(roleGroup => new {
                    id = roleGroup.Id,
                    groupName = roleGroup.GroupName,
                    groupDescription = roleGroup.Description,
                    department = roleGroup.DepartmentName,
                    groupScope = roleGroup.GroupScope,
                    groupCategory = roleGroup.GroupCategory,
                    groupType = roleGroup.GroupType,
                    isDeleted = roleGroup.IsDeleted,
                    isVerified = roleGroup.IsVerified,
                    isApproved = roleGroup.IsApproved,
                    createdOn = roleGroup.CreatedOn,
                    createdBy = roleGroup.CreatedBy,
                    modifiedOn = roleGroup.ModifiedOn,
                    modifiedBy = roleGroup.ModifiedBy,
                    permissionSets = roleGroup.PermissionSets?.Select(et => new {
                        id = et.Id,
                        setName = et.SetName,
                        setDescription = et.SetDescription,
                        isDeleted = et.IsDeleted,
                        createdOn = et.CreatedOn,
                        createdBy = et.CreatedBy,
                        modifiedOn = et.ModifiedOn,
                        modifiedBy = et.ModifiedBy,
                    }).ToList()
                }).ToList();

                //..handle null roles after projection
                foreach (var entity in pagedEntities)
                {
                    if (entity.permissionSets == null)
                    {
                        var anonymousType = entity.GetType();
                        var rolesProperty = anonymousType.GetProperty("permissionSets");
                        rolesProperty?.SetValue(entity, new List<object>());
                    }
                }

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system role groups: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Group Permissions Added", "User added system role group permissions", ActivityTypeDefaults.ROLE_GROUP_ADDED_WITH_PERMISSIONS, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANADDROLE")]
        public async Task<IActionResult> CreateRoleGroupPermissions([FromBody] RoleGroupViewModel request) {
            try {
                if (!ModelState.IsValid) {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null) {
                    return Ok(new { success = false, message = "Invalid role group data" });
                }

                var result = await _accessService.CreateRoleGroupWithPermissionsAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create role group" });

                var roleGroup = result.Data;
                return Ok(new {
                    success = roleGroup.Status,
                    message = roleGroup.Message,
                    data = new {
                        status = roleGroup.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Role Group Permissions Edited", "User updated system role group permissions", ActivityTypeDefaults.ROLE_GROUP_EDITED, "SystemRoleGroup")]
        [PermissionAuthorization(true, "CANVIEWROLES","CANMODIFYROLE")]
        public async Task<IActionResult> UpdateRoleGroupPermissions([FromBody] RoleGroupViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null) {
                    return Ok(new { success = false, message = "Invalid user data" });
                }

                var result = await _accessService.UpdateRoleGroupWithPermissionsAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update system role group" });

                var data = result.Data;
                return Ok(new {
                    success = data.Status,
                    message = data.Message,
                    data = new {
                        status = data.Status,
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error update system role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region System Permissions

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPermissions() {
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"PERMISSION LIST ERROR: Failed to retrieve current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }

                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.PERMISSIONS_RETRIVED.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all system permission
                var permissionData = await _accessService.GetPermissionsAsync(request);

                List<GrcPermissionResponse> permissions;
                if (permissionData.HasError)
                {
                    permissions = new();
                    Logger.LogActivity($"PERMISSION DATA ERROR: Failed to retrieve permission sets - {JsonSerializer.Serialize(permissionData)}");
                } else {
                    permissions = permissionData.Data.Data;
                    Logger.LogActivity($"PERMISSION DATA - {JsonSerializer.Serialize(permissions)}");
                }

                //..get ajax data
                List<object> listData = new();
                if (permissions.Any())
                {
                    listData = permissions.Select(permission => new {
                        id = permission.Id,
                        permissionName = permission.PermissionName,
                        permissionDescription = permission.PermissionDescription
                    }).Cast<object>().ToList();
                }

                return Json(new { data = listData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving permission sets: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region System Permission Sets

        [LogActivityResult("Permission Set Retrieved", "User retrieved Permission Set", ActivityTypeDefaults.PERMISSION_SET_RETRIEVED, "SystemPermissionSet")]
        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPermissionSet(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Permission Set Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _accessService.GetPermissionSetIdAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving Permission Set";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var setData = result.Data;
                var setRecord = new {
                    id = setData.Id,
                    setName = setData.SetName,
                    setDescription = setData.SetDescription,
                    isDeleted = setData.IsDeleted,
                    createdOn = setData.CreatedOn,
                    createdBy = setData.CreatedBy,
                    modifiedOn = setData.ModifiedOn,
                    modifiedBy = setData.ModifiedBy,
                    permissions = setData.Permissions,
                };

                return Ok(new { success = true, data = setRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving permission set : {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPermissionSets()
        {
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"PERMISSION SET LIST ERROR: Failed to retrieve current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                GrcRequest request = new()
                {
                    UserId = currentUser.UserId,
                    Action = Activity.PERMISSION_SET_RETRIVED.GetDescription(),
                    IPAddress = ipAddress,
                    EncryptFields = Array.Empty<string>(),
                    DecryptFields = Array.Empty<string>()
                };

                //..get list of all permission sets
                var setData = await _accessService.GetPermissionSetAsync(request);

                List<GrcPermissionSetResponse> permissionsets;
                if (setData.HasError)
                {
                    permissionsets = new();
                    Logger.LogActivity($"PERMISSION SET DATA ERROR: Failed to retrieve permission sets - {JsonSerializer.Serialize(setData)}");
                }
                else
                {
                    permissionsets = setData.Data.Data;
                    //Logger.LogActivity($"PERMISSION SET DATA - {JsonSerializer.Serialize(permissionsets)}");
                }

                //..get ajax data
                List<object> listData = new();
                if (permissionsets.Any())
                {
                    listData = permissionsets.Select(set => new {
                        id = set.Id,
                        setName = set.SetName,
                        setDescription = set.SetDescription,
                        isDeleted = set.IsDeleted,
                        createdOn = set.CreatedOn,
                        createdBy = set.CreatedBy,
                        modifiedOn = set.ModifiedOn,
                        modifiedBy = set.ModifiedBy
                    }).Cast<object>().ToList();
                }

                return Json(new { data = listData });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving permission sets: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [PermissionAuthorization(true, "CANVIEWROLES")]
        public async Task<IActionResult> GetPagedPermissionSets([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("SET DATA ERROR: Failed to get role record");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PERMISSION_SET_RETRIVED.GetDescription();

                var result = await _accessService.GetPagedPermissionSetAsync(request);
                PagedResponse<GrcPermissionSetResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcPermissionSetResponse>())
                    .Select(set => new {
                        id = set.Id,
                        setName = set.SetName,
                        setDescription = set.SetDescription,
                        isDeleted = set.IsDeleted,
                        createdOn = set.CreatedOn,
                        createdBy = set.CreatedBy,
                        modifiedOn = set.ModifiedOn,
                        modifiedBy = set.ModifiedBy
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving system roles: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Permission set Added", "User added Permission Set", ActivityTypeDefaults.PERMISSION_SET_ADDED, "SystemPermissionSet")]
        [PermissionAuthorization(true, "CANADDPERMISSIONSET")]
        public async Task<IActionResult> CreatePermissionSet([FromBody] GrcPermissionSetViewModel request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    string combinedErrors = string.Join("; ", errors);
                    return Ok(new
                    {
                        success = false,
                        message = $"Please correct these errors: {combinedErrors}",
                        data = (object)null
                    });
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid permission sets data" });
                }

                var result = await _accessService.CreatePermissionSetAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create permission sets" });

                var roleGroup = result.Data;
                return Ok(new
                {
                    success = roleGroup.Status,
                    message = roleGroup.Message,
                    data = new
                    {
                        status = roleGroup.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create role group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Permission set Edited", "User updated Permission Set", ActivityTypeDefaults.PERMISSION_SET_EDITED, "SystemPermissionSet")]
        [PermissionAuthorization(true, "CANMODIFYPERMISSIONSET")]
        public async Task<IActionResult> UpdatePermissionSet([FromBody] GrcPermissionSetViewModel request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid permission set data" });
                }

                var result = await _accessService.UpdatePermissionSetAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update permission set" });

                var data = result.Data;
                return Ok(new
                {
                    success = data.Status,
                    message = data.Message,
                    data = new
                    {
                        status = data.Status,
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error update permission set : {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Permission set Deleted", "User deleted Permission Set", ActivityTypeDefaults.PERMISSION_SET_DELETED, "SystemPermissionSet")]
        [PermissionAuthorization(true, "CANDELETEPERMISSIONSET")]
        public async Task<IActionResult> DeletePermissionSet(long id)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Permission Sety Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.PERMISSION_SET_DELETED.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _accessService.DeletePermissionSetAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete system permission set" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting permission set record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region Protected methods
        private static void SetSafeDate(IXLCell cell, DateTime? date) {
            if (!date.HasValue) {
                cell.Value = string.Empty;
                return;
            }

            var value = date.Value;
            //..excel-safe date range
            if (value.Year < 1900 || value.Year > 9999) {
                cell.Value = string.Empty;
                return;
            }

            cell.Value = value;
            cell.Style.DateFormat.Format = "dd-MMM-yyyy";
        }
        protected void Notify(string message, string title = "GRC NOTIFICATION", NotificationType type = NotificationType.Success) {
            var notificationMessage = new NotificationMessage() {
                Title = title,
                Message = message,
                Icon = type.GetEnumMemberValue(),
                Type  = type.GetEnumMemberValue()
            };

            TempData["Message"] = JsonSerializer.Serialize(notificationMessage); 
        }
        
        #endregion

    }
}
