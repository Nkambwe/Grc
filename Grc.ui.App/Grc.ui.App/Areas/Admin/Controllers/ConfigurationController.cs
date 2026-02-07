using ClosedXML.Excel;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Areas.Admin.Controllers {

    [Area("Admin")]
    public class ConfigurationController: AdminBaseController {
        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;
        private readonly ISupportDashboardFactory _dDashboardFactory;
        private readonly ISystemConfiguration _configService;
        public ConfigurationController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService,
                                 IErrorService errorService,
                                 IGrcErrorFactory errorFactory,
                                 ISupportDashboardFactory dDashboardFactory,
                                 ISystemConfiguration configService,
                                 SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, 
                  errorService, errorFactory, sessionManager) {
           _accessService = accessService;
            _authService = authService;
            _dDashboardFactory = dDashboardFactory;
            _configService = configService;
        }

        public async Task<IActionResult> Index() {
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

        public async Task<IActionResult> Organization() {
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

        public async Task<IActionResult> Branches()
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

        public async Task<IActionResult> UserData() {
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
        
        public async Task<IActionResult> UserGroups() {
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
        
        public async Task<IActionResult> DataEncryptions() {
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
        
        public async Task<IActionResult> BugReporter() {
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

        public async Task<IActionResult> NewBugs()
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

        public async Task<IActionResult> BugFixes()
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

        public async Task<IActionResult> BugProgress()
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
        public async Task<IActionResult> GetBug(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "System Error Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _configService.GetErrorAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving system error";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var response = result.Data;
                var bug = new {
                    id = response.Id,
                    severity = response.Severity ?? "CRITICAL",
                    error = response.Error ?? string.Empty,
                    source = response.Source ?? string.Empty,
                    status = response.Status ?? "OPEN",
                    assignedTo = response.AssignedTo ?? string.Empty,   
                    stackTrace = response.StackTrace ?? string.Empty,
                    createdOn = response.CreatedOn
                };

                return Ok(new { success = true, data = bug });
            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error retrieving task: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "POLICY-TASK", ex.StackTrace);
                return Redirect(Url.Action("PoliciesRegisters", "ComplianceSettings"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAllBugs([FromBody] BugListView request) {
            try {

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) {
                    Logger.LogActivity($"BUG LIST ERROR: Failed to get current user record - {JsonSerializer.Serialize(userResponse)}");
                    return Ok(new { last_page = 0, data = new List<object>() });
                }

                //..update with user data
                var currentUser = userResponse.Data;
                if (currentUser == null) {
                    Logger.LogActivity($"User record id null - {JsonSerializer.Serialize(userResponse)}");
                    //..session has expired
                    return Ok(new { last_page = 0, data = new List<object>() });
                }
                var result = await _configService.GetBugListAsync(request, currentUser.Id, currentUser.IPAddress);
                PagedResponse<GrcBugItemResponse> list = new();
                if (result.HasError) {
                    Logger.LogActivity($"BUG LIST ERROR: Failed to retrieve bug list - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"BUG LIST COUNT - {list.TotalCount}");
                }

                var buglist = list.Entities ?? new();
                if (buglist.Any()) {
                    var bugs = buglist.Select(bug => new {
                        id = bug.Id,
                        severity = bug.Severity,
                        error = bug.Error,
                        source = bug.Source,
                        status = bug.Status,
                        createdOn = bug.CreatedOn
                    }).ToList();

                    var totalPages = list.TotalPages <= 0 ? 1 : list.TotalPages;
                    return Ok(new { data = bugs, last_page = totalPages, total_records = list.TotalCount });
                }
                return Ok(new { last_page = 0, total_records = 0, data = Array.Empty<object>() });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving system bugs: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "CONFIGURATION-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetNewErrorList([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.BUG_STATUS.GetDescription();

                var result = await _configService.GetStatusBugsAsync(new GrcBugStatusListRequest {
                    UserId = request.UserId,
                    IPAddress = request.IPAddress,
                    Action = request.Action,
                    Status = "OPEN",
                    SearchTerm = request.SearchTerm,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection
                });

                PagedResponse<GrcBugResponse> list = null;
                if (result.HasError) {
                    list = new();
                    Logger.LogActivity($"BUG LIST ERROR: Failed to retrieve bug list - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"BUG LIST COUNT - {list.TotalCount}");

                }
                var pagedEntities = (list.Entities ?? new List<GrcBugResponse>())
                    .Select(bug => new {
                        id = bug.Id,
                        severity = bug.Severity,
                        error = bug.Error,
                        source = bug.Source,
                        status = bug.Status,
                        createdOn = bug.CreatedOn
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving system errors: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetClosedErrorList([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.BUG_STATUS.GetDescription();

                var result = await _configService.GetStatusBugsAsync(new GrcBugStatusListRequest {
                    UserId = request.UserId,
                    IPAddress = request.IPAddress,
                    Action = request.Action,
                    Status = "CLOSED",
                    SearchTerm = request.SearchTerm,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection
                });

                PagedResponse<GrcBugResponse> list = null;
                if (result.HasError) {
                    list = new();
                    Logger.LogActivity($"BUG LIST ERROR: Failed to retrieve bug list - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"BUG LIST COUNT - {list.TotalCount}");

                }
                var pagedEntities = (list.Entities ?? new List<GrcBugResponse>())
                    .Select(bug => new {
                        id = bug.Id,
                        severity = bug.Severity,
                        error = bug.Error,
                        source = bug.Source,
                        status = bug.Status,
                        createdOn = bug.CreatedOn
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving system errors: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetProgressErrorList([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("USER DATA ERROR: Failed to get user");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.BUG_STATUS.GetDescription();

                var result = await _configService.GetStatusBugsAsync(new GrcBugStatusListRequest {
                    UserId = request.UserId,
                    IPAddress = request.IPAddress,
                    Action = request.Action,
                    Status = "PROGRESS",
                    SearchTerm = request.SearchTerm,
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    SortBy = request.SortBy,
                    SortDirection = request.SortDirection
                });

                PagedResponse<GrcBugResponse> list = null;
                if (result.HasError) {
                    list = new();
                    Logger.LogActivity($"BUG LIST ERROR: Failed to retrieve bug list - {JsonSerializer.Serialize(result)}");
                } else {
                    list = result.Data;
                    Logger.LogActivity($"BUG LIST COUNT - {list.TotalCount}");

                }
                var pagedEntities = (list.Entities ?? new List<GrcBugResponse>())
                    .Select(bug => new {
                        id = bug.Id,
                        severity = bug.Severity,
                        error = bug.Error,
                        source = bug.Source,
                        status = bug.Status,
                        createdOn = bug.CreatedOn
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving system errors: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateError([FromBody] BugViewModel model) {
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

                var request = new GrcBugRequest {
                    ErrorMessage = model.ErrorMessage,
                    Source = model.Source,
                    Severity = model.Severity,
                    Status = "OPEN",
                    AssignedTo = model.AssignedTo,
                    StatckTrace = model.StackTrace,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_CREATE_TASK.GetDescription()
                };

                var result = await _configService.CreateErrorAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create error record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Record created successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error creating error record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateError([FromBody] BugViewModel model) {
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

                var request = new GrcBugRequest {
                    Id = model.Id,
                    ErrorMessage = model.ErrorMessage,
                    Source = model.Source,
                    Severity = model.Severity,
                    Status = model.Status,
                    AssignedTo = model.AssignedTo,
                    StatckTrace = model.StackTrace,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.COMPLIANCE_CREATE_TASK.GetDescription()
                };

                var result = await _configService.UpdateErrorAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update error record" });

                var created = result.Data;
                return Ok(new { success = true, message = "Record update successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updateing error record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangeErrorStatus([FromBody] BugViewModel model) {
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

                var request = new GrcBugStatusRequest {
                    RecordId = model.Id,
                    Status = model.Status,
                    MarkAsDeleted = false,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = Activity.BUG_STATUS.GetDescription()
                };

                var result = await _configService.ChangeErrorStausAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update error status" });

                var created = result.Data;
                return Ok(new { success = true, message = "Record update successfully" });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error updating error record: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "SYSTEM-SUPPORT", ex.StackTrace);
                return Ok(new { success = false, message = "An unexpected error occurred" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ExportBugList() {

            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new GrcRequest {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                EncryptFields = Array.Empty<string>(),
                DecryptFields = Array.Empty<string>(),
                Action = Activity.EXPORT_BUG_LIST.GetDescription()
            };

            var result = await _configService.GetNewBugsAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = result.Error.Message ?? "Failed to retrieve system error list" });

            var data = result.Data.Data ?? new List<GrcBugResponse>();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("GRC SYSTEM ERRORS");

            //..headers
            string[] headers = {
                "ERROR MESSAGE",
                "ERROR SOURCE",
                "STACKTRACE",
                "SEVERITY"
            };

            for (int col = 0; col < headers.Length; col++) {
                ws.Cell(1, col + 1).Value = headers[col];
            }

            // Header styling
            var header = ws.Range(1, 1, 1, headers.Length);
            header.Style.Font.Bold = true;
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;
            header.Style.Border.BottomBorder = XLBorderStyleValues.Thick;

            ws.Row(1).Height = 30;

            // Data
            int row = 2;
            foreach (var p in data) {
                ws.Cell(row, 1).Value = p.Error;
                ws.Cell(row, 2).Value = p.Source;
                ws.Cell(row, 3).Value = p.StackTrace;
                ws.Cell(row, 4).Value = p.Severity;

                row++;
            }

            int lastDataRow = row - 1;

            // Wrap text for long columns
            ws.Column(1).Style.Alignment.WrapText = true; 
            ws.Column(3).Style.Alignment.WrapText = true; 

            // Background styling
            ws.Range(2, 1, lastDataRow, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#F1F5F9");
            ws.Range(2, 3, lastDataRow, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#F9FAFB");

            //..severity formatting
            var severityRange = ws.Range(2, 4, lastDataRow, 4);
            severityRange.Style.Font.Bold = true;
            severityRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            severityRange.AddConditionalFormat().WhenEquals("HIGH").Fill.SetBackgroundColor(XLColor.Red);
            severityRange.AddConditionalFormat().WhenEquals("MEDIUM").Fill.SetBackgroundColor(XLColor.Orange);
            severityRange.AddConditionalFormat().WhenEquals("LOW").Fill.SetBackgroundColor(XLColor.LightGreen);

            //..borders
            ws.Range(1, 1, lastDataRow, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            ws.Range(1, 1, lastDataRow, 4).Style.Border.InsideBorder = XLBorderStyleValues.Hair;

            // Column widths
            ws.Column(1).Width = 50;  
            ws.Column(2).Width = 28;  
            ws.Column(3).Width = 75;  
            ws.Column(4).Width = 12;  

            // Auto row height (this is key)
            ws.Rows(2, lastDataRow).AdjustToContents();

            // Header filters + freeze
            ws.Range(1, 1, 1, headers.Length).SetAutoFilter();
            ws.SheetView.FreezeRows(1);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"SYSTEM-ERRORS-{DateTime.Today:yyyy-MM}.xlsx"
            );
        }

        public async Task<IActionResult> SystemActivity() {
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

    }
}
