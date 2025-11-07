using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
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
                if (userResponse.HasError) Logger.LogActivity("OPERATION PROCESSES DATA ERROR: Failed to operation processes, procedures or guides record");

                var currentUser = userResponse.Data;
                request.UserId = currentUser.UserId;
                request.IPAddress = ipAddress;
                request.Action = Activity.PROCESSES_RETRIEVE_PROCESS.GetDescription();

                var result = await _processService.GetProcessRegistersActAsync(request);
                PagedResponse<GrcProcessRegisterResponse> list = result.Data ?? new();

                var pagedEntities = (list.Entities ?? new List<GrcProcessRegisterResponse>())
                    .Select(process => new {
                        id = process.Id,
                        processName = process.ProcessName ?? string.Empty,
                        description = process.Description ?? string.Empty,
                        CurrentVersion = process.CurrentVersion ?? "0.0.0",
                        fileName = process.FileName,
                        originalOnFile = process.OriginalOnFile,
                        processStatus = process.ProcessStatus ?? "UNDEFINED",
                        approvalStatus = process.ApprovalStatus,
                        approvalComment = process.ApprovalComment ?? string.Empty,
                        onholdReason = process.OnholdReason ?? string.Empty,
                        typeId = process.TypeId,
                        typeName = process.TypeName ?? string.Empty,
                        unitId = process.UnitId,
                        unitName = process.UnitName ?? string.Empty,
                        ownerId = process.OwnerId,
                        ownerName = process.OwnerName ?? string.Empty,
                        assigneedId = process.ResponsibilityId,
                        assigneeName = process.Responsible ?? string.Empty,
                        isDeleted = process.IsDeleted,
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
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Ok(new { last_page = 0, data = new List<object>() });
            }
        }

        [HttpPost]
        [LogActivityResult("Retrieve Process Register", "User retrieved operations process register", ActivityTypeDefaults.PROCESSES_RETRIEVE_PROCESS, "OperationProcess")]
        public async Task<IActionResult> GetProcessRegister(long id) {
            return Ok();
        }

        [HttpPost]
        [LogActivityResult("Create Process Register", "User created operations process register", ActivityTypeDefaults.PROCESSES_CREATE_PROCESS, "OperationProcess")]
        public async Task<IActionResult> CreateProcessRegister([FromBody] ProcessViewModel request) {
            return Ok();
        }

        [HttpPost]
        [LogActivityResult("Update Process Register", "User update operations process register", ActivityTypeDefaults.PROCESSES_EDITED_PROCESS, "OperationProcess")]
        public async Task<IActionResult> UpdateProcessRegister([FromBody] ProcessViewModel request) {
            return Ok();
        }

        [HttpPost]
        [LogActivityResult("Delete Process Register", "User deleted operations process register", ActivityTypeDefaults.PROCESSES_DELETED_PROCESS, "OperationProcess")]
        public async Task<IActionResult> DeleteProcessRegister() {
            return Ok();
        }

        [HttpPost]
        [LogActivityResult("Export Process Registers", "User exported process registers to excel", ActivityTypeDefaults.PROCESSES_EXPORT_PROCESS, "OperationProcess")]
        public async Task<IActionResult> ExportProcessRegisterTable([FromBody] List<ProcessViewModel> data) {
            return Ok();
        }

        [HttpPost]
        [LogActivityResult("Export Process Registers", "User exported process registers to excel", ActivityTypeDefaults.PROCESSES_EXPORT_PROCESS, "OperationProcess")]
        public async Task<IActionResult> ExportProcessRegisterAll() {
            return Ok();
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

        #endregion

        #region Process Pending

        public async Task<IActionResult> Pending() {
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
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        #endregion

    }
}
