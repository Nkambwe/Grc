using ClosedXML.Excel;
using Grc.ui.App.Defaults;
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

namespace Grc.ui.App.Areas.Operations.Controllers {

    [Area("Operations")]
    public class OperationWorkflowController : OperationsBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IOperationsDashboardFactory _dDashboardFactory;
        private readonly IProcessesService _processService;
        public OperationWorkflowController(IApplicationLoggerFactory loggerFactory, 
                                           IEnvironmentProvider environment,
                                           IWebHelper webHelper, 
                                           ILocalizationService localizationService,
                                           IErrorService errorService, 
                                           IOperationsDashboardFactory dDashboardFactory,
                                           IAuthenticationService authService,
                                           IGrcErrorFactory errorFactory,
                                           IProcessesService processService,
                                           SessionManager sessionManager) 
                                        : base(loggerFactory, environment, webHelper, localizationService, 
                                              errorService, errorFactory, sessionManager) {
                                        _dDashboardFactory = dDashboardFactory;
                                        _authService = authService;
                                        _processService = processService;
        }

        #region Process Registers

        public async Task<IActionResult> RegisterProcess() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-WORKFLOW-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> ProcessRegisterList([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("OPERATION PROCESSES DATA ERROR: Failed to retrieve operation processes, procedures or guides record");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription();

                var result = await _processService.GetProcessRegistersAsync(request);
                PagedResponse<GrcProcessRegisterResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcProcessRegisterResponse>())
                    .Select(process => new {
                        id = process.Id,
                        processName = process.ProcessName ?? string.Empty,
                        description = process.Description ?? string.Empty,
                        currentVersion = process.CurrentVersion ?? "0.0.0",
                        fileName = process.FileName,
                        originalOnFile = process.OriginalOnFile,
                        processStatus = process.ProcessStatus ?? "UNDEFINED",
                        onholdReason = process.OnholdReason ?? string.Empty,
                        comment = process.Comments ?? string.Empty,
                        typeId = process.TypeId,
                        typeName = process.TypeName ?? string.Empty,
                        unitId = process.UnitId,
                        unitName = process.UnitName ?? string.Empty,
                        ownerId = process.OwnerId,
                        ownerName = process.OwnerName ?? string.Empty,
                        assigneedId = process.ResponsibilityId,
                        assigneeName = process.Responsible ?? string.Empty,
                        isDeleted = process.IsDeleted,
                        isLockProcess = process.IsLockProcess,
                        effectiveDate = process.EffectiveDate,
                        createdOn = process.CreatedOn,
                        createdBy = process.CreatedBy ?? string.Empty,
                        modifiedOn = process.ModifiedOn ?? process.CreatedOn,
                        modifiedBy = process.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving Operation processes: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [LogActivityResult("Retrieve Process Register", "User retrieved operations process register", ActivityTypeDefaults.PROCESSES_RETRIEVE_PROCESS, "OperationProcess")]
        public async Task<IActionResult> GetProcessRegister(long id) {
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
                    return BadRequest(new { success = false, message = "Process Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _processService.GetProcessRegisterAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving process";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var process = result.Data;
                var processRecord = new
                {
                    id = process.Id,
                    processName = process.ProcessName ?? string.Empty,
                    description = process.Description ?? string.Empty,
                    currentVersion = process.CurrentVersion ?? "0.0.0",
                    fileName = process.FileName,
                    originalOnFile = process.OriginalOnFile,
                    processStatus = process.ProcessStatus ?? "UNDEFINED",
                    onholdReason = process.OnholdReason ?? string.Empty,
                    comment = process.Comments ?? string.Empty,
                    typeId = process.TypeId,
                    typeName = process.TypeName ?? string.Empty,
                    unitId = process.UnitId,
                    unitName = process.UnitName ?? string.Empty,
                    ownerId = process.OwnerId,
                    ownerName = process.OwnerName ?? string.Empty,
                    assigneedId = process.ResponsibilityId,
                    assigneeName = process.Responsible ?? string.Empty,
                    needsBranchOperations = process.NeedsBranchReview,
                    needsCreditReview = process.NeedsCreditReview,
                    needsTreasuryReview = process.NeedsTreasuryReview,
                    needsFintechReview = process.NeedsFintechReview,
                    isAssigned = process.IsAssigned,
                    hodApprovalOn = process.Approvals?.HeadOfDepartmentEnd,
                    hodApprovalStatus = process.Approvals?.HeadOfDepartmentStatus ?? "PENDING",
                    hoApprovalComment = process.Approvals?.HeadOfDepartmentComment ?? string.Empty,
                    riskApprovalOn = process.Approvals?.RiskEnd,
                    riskApprovalStatus = process.Approvals?.RiskStatus ?? "PENDING",
                    riskApprovalComment = process.Approvals?.RiskComment ?? string.Empty,
                    complianceApprovalOn = process.Approvals?.ComplianceEnd,
                    complianceApprovalStatus = process.Approvals?.ComplianceStatus ?? "PENDING",
                    complianceApprovalComment = process.Approvals?.ComplianceComment ?? string.Empty,
                    branchOpsApprovalOn = process.Approvals?.BranchOperationsStatusEnd,
                    branchOpsApprovalStatus = process.Approvals?.BranchOperationsStatus ?? "PENDING",
                    branchOpsApprovalComment = process.Approvals?.BranchManagerComment ?? string.Empty,
                    creditApprovalOn = process.Approvals?.CreditEnd,
                    creditApprovalStatus = process.Approvals?.CreditStatus ?? "PENDING",
                    creditApprovalComment = process.Approvals?.CreditComment ?? string.Empty,
                    treasuryApprovalOn = process.Approvals?.TreasuryEnd,
                    treasuryApprovalStatus = process.Approvals?.TreasuryStatus ?? "PENDING",
                    treasuryApprovalComment = process.Approvals?.TreasuryComment ?? string.Empty,
                    fintechApprovalOn = process.Approvals?.FintechEnd,
                    fintechApprovalStatus = process.Approvals?.FintechStatus ?? "PENDING",
                    fintechApprovalComment = process.Approvals?.FintechComment ?? string.Empty,
                    isDeleted = process.IsDeleted,
                    isLockProcess = process.IsLockProcess,
                    effectiveDate = process.EffectiveDate,
                    createdOn = process.CreatedOn,
                    createdBy = process.CreatedBy ?? string.Empty,
                    modifiedOn = process.ModifiedOn ?? process.CreatedOn,
                    modifiedBy = process.ModifiedBy ?? string.Empty
                };

                return Ok(new { success = true, data = processRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-REGISTER-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Create Process Register", "User created operations process register", ActivityTypeDefaults.PROCESSES_CREATE_PROCESS, "OperationProcess")]
        public async Task<IActionResult> CreateProcessRegister([FromBody] ProcessViewModel request) {
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
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid process data" });
                }

                var result = await _processService.CreateProcessAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create process record" });

                var process = result.Data;
                return Ok(new
                {
                    success = process.Status,
                    message = process.Message,
                    data = new
                    {
                        status = process.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create process record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Process Register", "User update operations process register", ActivityTypeDefaults.PROCESSES_EDITED_PROCESS, "OperationProcess")]
        public async Task<IActionResult> UpdateProcessRegister([FromBody] ProcessViewModel request) {
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

                var result = await _processService.UpdateProcessAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update process record" });

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
                Logger.LogActivity($"Error update process record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Process Register", "User deleted operations process register", ActivityTypeDefaults.PROCESSES_DELETED_PROCESS, "OperationProcess")]
        public async Task<IActionResult> DeleteProcessRegister(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Process Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.PROCESSES_DELETED_PROCESS.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _processService.DeleteProcessAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete process record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting process record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Export Process Registers", "User exported process registers to excel", ActivityTypeDefaults.PROCESSES_EXPORT_PROCESS, "OperationProcess")]
        public async Task<IActionResult> ExportProcessRegisterAll() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
            if (userResponse.HasError || userResponse.Data == null)
                return Ok(new { success = false, message = "Unable to resolve current user" });

            var request = new TableListRequest
            {
                UserId = userResponse.Data.UserId,
                IPAddress = ipAddress,
                PageIndex = 1,
                PageSize = int.MaxValue,
                Action = Activity.PROCESS_EXPORT.GetDescription()
            };

            var result = await _processService.GetProcessRegistersAsync(request);
            if (result.HasError || result.Data == null)
                return Ok(new { success = false, message = "Failed to retrieve operation process" });

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("PROCESS UNIVERSE");

            ws.Cell(1, 1).Value = "PROCESS NAME";
            ws.Cell(1, 2).Value = "UNIT";
            ws.Cell(1, 3).Value = "PROCESS MGR";
            ws.Cell(1, 4).Value = "PROCESS OWNER";
            ws.Cell(1, 5).Value = "PROCESS DESCRIPTION";
            ws.Cell(1, 6).Value = "LAST UPDATED";
            ws.Cell(1, 7).Value = "STATUS";

            int row = 2;
            foreach (var p in result.Data.Entities)
            {
                ws.Cell(row, 1).Value = p.ProcessName;
                ws.Cell(row, 2).Value = p.UnitName;
                ws.Cell(row, 3).Value = p.Responsible;
                ws.Cell(row, 4).Value = p.OwnerName;
                ws.Cell(row, 5).Value = p.Description ?? string.Empty;
                ws.Cell(row, 6).Value = p.LastReviewDate.ToString("MM-dd-yyyy");
                ws.Cell(row, 7).Value = p.ProcessStatus ?? "DRAFT";

                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "PROCESS_UNIVERSE.xlsx");
        }

        #endregion

        #region Process Groups

        public async Task<IActionResult> GroupProcesses() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-WORKFLOW-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [LogActivityResult("Retrieve Process Group", "User retrieved operations process group", ActivityTypeDefaults.PROCESSES_RETRIEVE_GROUP, "ProcessGroup")]
        public async Task<IActionResult> GetProcessGroup(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Process group Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _processService.GetProcessGroupAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving process group";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var group = result.Data;
                var groupRecord = new {
                    id = group.Id,
                    groupName = group.GroupName ?? string.Empty,
                    groupDescription = group.GroupDescription ?? string.Empty,
                    isDeleted = group.IsDeleted,
                    createdOn = group.CreatedOn,
                    createdBy = group.CreatedBy ?? string.Empty,
                    modifiedOn = group.ModifiedOn ?? group.CreatedOn,
                    modifiedBy = group.ModifiedBy ?? string.Empty
                };

                return Ok(new { success = true, data = groupRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-REGISTER-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        public async Task<IActionResult> ProcessGroupList([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("PROCESS GROUPS DATA ERROR: Failed to retrieve process groups");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESS_GROUPS_RETRIVED.GetDescription();

                var result = await _processService.GetProcessGroupsAsync(request);
                PagedResponse<GrcProcessGroupResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcProcessGroupResponse>())
                    .Select(group => new {
                        id = group.Id,
                        groupName = group.GroupName ?? string.Empty,
                        groupDescription = group.GroupDescription ?? string.Empty,
                        isDeleted = group.IsDeleted,
                        createdOn = group.CreatedOn,
                        createdBy = group.CreatedBy ?? string.Empty,
                        modifiedOn = group.ModifiedOn ?? group.CreatedOn,
                        modifiedBy = group.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Create Process group", "User created operations process group", ActivityTypeDefaults.PROCESSES_CREATE_GROUP, "ProcessGroup")]
        public async Task<IActionResult> CreateProcessGroup([FromBody] ProcessGroupViewModel request) {
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
                    return Ok(new { success = false, message = "Invalid process group data" });
                }

                var result = await _processService.CreateProcessGroupAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create process group" });

                var process = result.Data;
                return Ok(new
                {
                    success = process.Status,
                    message = process.Message,
                    data = new
                    {
                        status = process.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create process group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Process group", "User update operations process group", ActivityTypeDefaults.PROCESSES_EDITED_GROUP, "ProcessGroup")]
        public async Task<IActionResult> UpdateProcessGroup([FromBody] ProcessGroupViewModel request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                var currentUser = userResponse.Data;
                if (request == null) {
                    return Ok(new { success = false, message = "Invalid user data" });
                }

                var result = await _processService.UpdateProcessGroupAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update process group" });

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
                Logger.LogActivity($"Error update process group: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Process Group", "User deleted operations process group", ActivityTypeDefaults.PROCESSES_DELETED_GROUP, "ProcessGroup")]
        public async Task<IActionResult> DeleteProcessGroup(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Process Group Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new()
                {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.PROCESS_GROUP_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _processService.DeleteProcessGroupAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete process group record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting process group record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region Process Tags

        public async Task<IActionResult> TagProcesses() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-WORKFLOW-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [LogActivityResult("Retrieve Process Tag", "User retrieved operations process tag", ActivityTypeDefaults.PROCESSES_RETRIEVE_TAG, "ProcessTag")]
        public async Task<IActionResult> GetProcessTag(long id) {
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
                    return BadRequest(new { success = false, message = "Process tag Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _processService.GetProcessTagAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving process tag";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var tag = result.Data;
                var tagRecord = new
                {
                    id = tag.Id,
                    tagName = tag.TagName ?? string.Empty,
                    tagDescription = tag.TagDescription ?? string.Empty,
                    isDeleted = tag.IsDeleted,
                    createdOn = tag.CreatedOn,
                    createdBy = tag.CreatedBy ?? string.Empty,
                    modifiedOn = tag.ModifiedOn ?? tag.CreatedOn,
                    modifiedBy = tag.ModifiedBy ?? string.Empty
                };

                return Ok(new { success = true, data = tagRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process tag: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-REGISTER-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Create Process Tag", "User created operations process tag", ActivityTypeDefaults.PROCESSES_CREATE_TAG, "ProcessTag")]
        public async Task<IActionResult> CreateProcessTag([FromBody] ProcessTagViewModel request) {
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
                if (request == null)
                {
                    return Ok(new { success = false, message = "Invalid process tag data" });
                }

                var result = await _processService.CreateProcessTagAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to create process tag" });

                var process = result.Data;
                return Ok(new
                {
                    success = process.Status,
                    message = process.Message,
                    data = new
                    {
                        status = process.Status,
                    }
                });

            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error create process tag: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        public async Task<IActionResult> ProcessTagList([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("PROCESS TAGS DATA ERROR: Failed to retrieve process tags");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESS_GROUPS_RETRIVED.GetDescription();

                var result = await _processService.GetProcessTagsAsync(request);
                PagedResponse<GrcProcessTagResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcProcessTagResponse>())
                    .Select(tag => new {
                        id = tag.Id,
                        tagName = tag.TagName ?? string.Empty,
                        tagDescription = tag.TagDescription ?? string.Empty,
                        isDeleted = tag.IsDeleted,
                        createdOn = tag.CreatedOn,
                        createdBy = tag.CreatedBy ?? string.Empty,
                        modifiedOn = tag.ModifiedOn ?? tag.CreatedOn,
                        modifiedBy = tag.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process tags: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Update Process tag", "User update operations process tag", ActivityTypeDefaults.PROCESSES_EDITED_TAG, "ProcessTag")]
        public async Task<IActionResult> UpdateProcessTag([FromBody] ProcessTagViewModel request) {
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

                var result = await _processService.UpdateProcessTagAsync(request, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to update process tag record" });

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
                Logger.LogActivity($"Error update process group record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Delete Process tag", "User deleted operations process tag", ActivityTypeDefaults.PROCESSES_DELETED_TAG, "ProcessTag")]
        public async Task<IActionResult> DeleteProcessTag(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null)
                    return Ok(new { success = false, message = "Unable to resolve current user" });

                if (id == 0) return BadRequest(new { success = false, message = "Process Tag Id is required" });

                var currentUser = userResponse.Data;
                GrcIdRequest request = new() {
                    RecordId = id,
                    UserId = currentUser.UserId,
                    Action = Activity.PROCESS_TAG_DELETE.GetDescription(),
                    IPAddress = ipAddress,
                    IsDeleted = true
                };

                var result = await _processService.DeleteProcessTagAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to delete process tag record" });

                return Ok(new { success = result.Data.Status, message = result.Data.Message });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error deleting process tag record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        #endregion

        #region Process TAT Report

        public async Task<IActionResult> TATReport()
        {
            var model = new OperationsDashboardModel();
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-WORKFLOW-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-WORKFLOW-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [LogActivityResult("Retrieve Process TAT", "User retrieved operations process TAT", ActivityTypeDefaults.PROCESSES_RETRIEVE_GROUP, "ProcessApproval")]
        public async Task<IActionResult> GetProcessTat(long id) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (id == 0) {
                    return BadRequest(new { success = false, message = "Process group Id is required", data = new { } });
                }

                var currentUser = userResponse.Data;
                var result = await _processService.GetProcessTatAsync(id, currentUser.UserId, ipAddress);
                if (result.HasError || result.Data == null) {
                    var errMsg = result.Error?.Message ?? "Error occurred while retrieving process";
                    Logger.LogActivity(errMsg);
                    return Ok(new { success = false, message = errMsg, data = new { } });
                }

                var tat = result.Data;
                var tatRecord = new {
                    id = tat.Id,
                    processName = tat.ProcessName ?? string.Empty,
                    processStatus = tat.ProcessStatus ?? string.Empty,
                    requestDate = tat.RequestDate,
                    hodStartdate = tat.HodStartdate,
                    hodEnddate = tat.HodEnddate,
                    HodStatus = tat.HodStatus ?? "PENDING",
                    hodComment = tat.HodComment ?? string.Empty,
                    hodCount = tat.HodCount,
                    riskStartdate = tat.RiskStartdate,
                    riskEnddate = tat.RiskEnddate,
                    riskStatus = tat.RiskStatus ?? "PENDING",
                    riskComment = tat.RiskComment ?? string.Empty,
                    riskCount = tat.RiskCount,
                    complianceStartdate = tat.ComplianceStartdate,
                    complianceEnddate = tat.ComplianceEnddate,
                    complianceStatus = tat.ComplianceStatus ?? "PENDING",
                    complianceComment = tat.ComplianceComment ?? string.Empty,
                    complianceCount = tat.ComplianceCount,
                    needBropsApproval = tat.NeedBropsApproval,
                    bropStartdate = tat.BropStartdate,
                    bropEnddate = tat.BropEnddate,
                    bropStatus = tat.BropStatus ?? "PENDING",
                    bropComment = tat.BropComment ?? string.Empty,
                    bopCount = tat.BopCount,
                    needTreasuryApproval = tat.NeedTreasuryApproval,
                    treasuryStartdate = tat.TreasuryStartdate,
                    treasuryEnddate = tat.TreasuryEnddate,
                    treasuryStatus = tat.TreasuryStatus ?? "PENDING",
                    treasuryComment = tat.TreasuryComment ?? string.Empty,
                    treasuryCount = tat.TreasuryCount,
                    needFintechApproval = tat.NeedFintechApproval,
                    fintechStartdate = tat.FintechStartdate,
                    fintechEnddate = tat.FintechEnddate,
                    fintechStatus = tat.FintechStatus ?? "PENDING",
                    fintechComment = tat.FintechComment ?? string.Empty,
                    fintechCount = tat.FintechCount,
                    needCreditApproval = tat.NeedCreditApproval,
                    creditStartdate = tat.CreditStartdate,
                    creditEnddate = tat.CreditEnddate,
                    creditStatus = tat.CreditStatus ?? "PENDING",
                    creditComment = tat.CreditComment ?? string.Empty,
                    creditCount = tat.CreditCount,
                    createdOn = tat.CreatedOn,
                    createdBy = tat.CreatedBy ?? string.Empty,
                    modifiedOn = tat.ModifiedOn ?? tat.CreatedOn,
                    modifiedBy = tat.ModifiedBy ?? string.Empty
                };

                return Ok(new { success = true, data = tatRecord });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process tat: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Json(new { results = new List<object>() });
            }
        }

        public async Task<IActionResult> ProcessTATList([FromBody] TableListRequest request) {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("PROCESS TAT DATA ERROR: Failed to retrieve process TAT data");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESS_TATDATA_RETRIVED.GetDescription();

                var result = await _processService.GetProcessTatAsync(request);
                PagedResponse<GrcProcessTatResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcProcessTatResponse>())
                    .Select(tat => new {
                        id = tat.Id,
                        processName = tat.ProcessName ?? string.Empty,
                        processStatus = tat.ProcessStatus ?? string.Empty,
                        requestDate = tat.RequestDate,
                        hodCount = tat.HodCount,
                        riskCount = tat.RiskCount,
                        complianceCount = tat.ComplianceCount,
                        requireBop = tat.NeedBropsApproval,
                        bopCount = tat.BopCount,
                        requireTreasury = tat.NeedTreasuryApproval,
                        treasuryCount = tat.TreasuryCount,
                        requireFintech = tat.NeedFintechApproval,
                        fintechCount = tat.FintechCount,
                        requireCredit = tat.NeedCreditApproval,
                        creditCount = tat.CreditCount,
                        totalCount = tat.TotalCount,
                        createdOn = tat.CreatedOn,
                        createdBy = tat.CreatedBy ?? string.Empty,
                        modifiedOn = tat.ModifiedOn ?? tat.CreatedOn,
                        modifiedBy = tat.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving process tags: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Process Approvals

        public async Task<IActionResult> Approvals() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-WORKFLOW-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }
        public async Task<IActionResult> ProcessApprovalList([FromBody] TableListRequest request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("PROCESS APPROVAL DATA ERROR: Failed to retrieve process approvals");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription();

                var result = await _processService.GetProcessApprovalStatusAsync(request);
                PagedResponse<GrcProcessApprovalStatusResponse> list = result.Data ?? new();
                var pagedEntities = (list.Entities ?? new List<GrcProcessApprovalStatusResponse>())
                    .Select(approval => new {
                        id = approval.Id,
                        processName = approval.ProcessName ?? string.Empty,
                        requestDate = approval.RequestDate,
                        hodStatus = string.IsNullOrWhiteSpace(approval.HodStatus) ? "PENDING": approval.HodStatus,
                        riskStatus = (string.IsNullOrWhiteSpace(approval.HodStatus) || approval.HodStatus == "PENDING") ? "NOT STARTED" : (string.IsNullOrWhiteSpace(approval.RiskStatus) ? "PENDING" : approval.RiskStatus),
                        complianceStatus = (string.IsNullOrWhiteSpace(approval.RiskStatus) || approval.RiskStatus == "PENDING") ? "NOT STARTED" : approval.ComplianceStatus ?? "PENDING",
                        requiresBopApproval = approval.RequiresBopApproval,
                        bopStatus = (approval.RequiresBopApproval && (string.IsNullOrWhiteSpace(approval.ComplianceStatus) || approval.ComplianceStatus == "PENDING")) ? "NOT STARTED" :
                        (approval.RequiresBopApproval && (!string.IsNullOrWhiteSpace(approval.ComplianceStatus) || approval.ComplianceStatus != "PENDING")) ? (string.IsNullOrWhiteSpace(approval.BopStatus) ? "PENDING" : approval.BopStatus) :
                        "NA",
                        requiresCreditApproval = approval.RequiresCreditApproval,
                        creditStatus = (approval.RequiresCreditApproval && (string.IsNullOrWhiteSpace(approval.BopStatus) || approval.BopStatus == "PENDING")) ? "NOT STARTED" :
                        (approval.RequiresCreditApproval && (!string.IsNullOrWhiteSpace(approval.BopStatus) || approval.BopStatus != "PENDING")) ? (string.IsNullOrWhiteSpace(approval.CreditStatus) ? "PENDING" : approval.CreditStatus) :
                        "NA",
                        requiresTreasuryApproval = approval.RequiresTreasuryApproval,
                        treasuryStatus = (approval.RequiresTreasuryApproval && (string.IsNullOrWhiteSpace(approval.CreditStatus) || approval.CreditStatus == "PENDING")) ? "NOT STARTED" :
                        (approval.RequiresTreasuryApproval && (!string.IsNullOrWhiteSpace(approval.CreditStatus) || approval.CreditStatus != "PENDING")) ? (string.IsNullOrWhiteSpace(approval.TreasuryStatus) ? "PENDING" : approval.TreasuryStatus) :
                        "NA",
                        requiresFintechApproval = approval.RequiresFintechApproval,
                        fintechStatus = (approval.RequiresTreasuryApproval && (string.IsNullOrWhiteSpace(approval.TreasuryStatus) || approval.TreasuryStatus == "PENDING")) ? "NOT STARTED" :
                        (approval.RequiresTreasuryApproval && (!string.IsNullOrWhiteSpace(approval.TreasuryStatus) || approval.TreasuryStatus != "PENDING")) ? (string.IsNullOrWhiteSpace(approval.FintechStatus) ? "PENDING" : approval.FintechStatus) :
                        "NA",
                        processId = approval.ProcessId,

                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving Operation processes: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region New Process

        public async Task<IActionResult> NewProcess() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-WORKFLOW-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> ProcessNewList([FromBody] TableListRequest request)
        {
            try
            {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("NEW PROCESSES DATA ERROR: Failed to retrieve new processes");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription();

                var result = await _processService.GetNewProcessAsync(request);
                PagedResponse<GrcProcessRegisterResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcProcessRegisterResponse>())
                    .Select(process => new {
                        id = process.Id,
                        processName = process.ProcessName ?? string.Empty,
                        description = process.Description ?? string.Empty,
                        currentVersion = process.CurrentVersion ?? "0.0.0",
                        fileName = process.FileName,
                        originalOnFile = process.OriginalOnFile,
                        processStatus = process.ProcessStatus ?? "UNDEFINED",
                        onholdReason = process.OnholdReason ?? string.Empty,
                        comment = process.Comments ?? string.Empty,
                        typeId = process.TypeId,
                        typeName = process.TypeName ?? string.Empty,
                        unitId = process.UnitId,
                        unitName = process.UnitName ?? string.Empty,
                        ownerId = process.OwnerId,
                        ownerName = process.OwnerName ?? string.Empty,
                        assigneedId = process.ResponsibilityId,
                        assigneeName = process.Responsible ?? string.Empty,
                        isDeleted = process.IsDeleted,
                        isLockProcess = process.IsLockProcess,
                        effectiveDate = process.EffectiveDate,
                        createdOn = process.CreatedOn,
                        createdBy = process.CreatedBy ?? string.Empty,
                        modifiedOn = process.ModifiedOn ?? process.CreatedOn,
                        modifiedBy = process.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);

                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving Operation processes: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

        #region Process Revisions

        public async Task<IActionResult> Revisions() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-WORKFLOW-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        public async Task<IActionResult> ProcessReviewList([FromBody] TableListRequest request) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError) Logger.LogActivity("REVIEW PROCESSES DATA ERROR: Failed to retrieve process reviews");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription();

                var result = await _processService.GetReviewProcessAsync(request);
                PagedResponse<GrcProcessRegisterResponse> list = result.Data ?? new();
                var pagedEntities = (list.Entities ?? new List<GrcProcessRegisterResponse>())
                    .Select(process => new {
                        id = process.Id,
                        processName = process.ProcessName ?? string.Empty,
                        description = process.Description ?? string.Empty,
                        currentVersion = process.CurrentVersion ?? "0.0.0",
                        fileName = process.FileName,
                        originalOnFile = process.OriginalOnFile,
                        processStatus = process.ProcessStatus ?? "UNDEFINED",
                        onholdReason = process.OnholdReason ?? string.Empty,
                        comment = process.Comments ?? string.Empty,
                        typeId = process.TypeId,
                        typeName = process.TypeName ?? string.Empty,
                        unitId = process.UnitId,
                        unitName = process.UnitName ?? string.Empty,
                        ownerId = process.OwnerId,
                        ownerName = process.OwnerName ?? string.Empty,
                        assigneedId = process.ResponsibilityId,
                        assigneeName = process.Responsible ?? string.Empty,
                        isDeleted = process.IsDeleted,
                        isLockProcess = process.IsLockProcess,
                        effectiveDate = process.EffectiveDate,
                        createdOn = process.CreatedOn,
                        createdBy = process.CreatedBy ?? string.Empty,
                        modifiedOn = process.ModifiedOn ?? process.CreatedOn,
                        modifiedBy = process.ModifiedBy ?? string.Empty
                    }).ToList();

                var totalPages = (int)Math.Ceiling((double)list.TotalCount / list.Size);
                return Ok(new { last_page = totalPages, total_records = list.TotalCount, data = pagedEntities });
            }
            catch (Exception ex)
            {
                Logger.LogActivity($"Error retrieving Operation processes: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "PROCESS-WORKFLOW-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        #endregion

    }
}
