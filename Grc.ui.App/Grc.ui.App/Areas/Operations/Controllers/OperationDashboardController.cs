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
using System.Text.Json;

namespace Grc.ui.App.Areas.Operations.Controllers {

    [Area("Operations")]
    public class OperationDashboardController : OperationsBaseController {

        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;
        private readonly IOperationsDashboardFactory _operationsDashboardFactory;
        private readonly ISupportDashboardFactory _dDashboardFactory;
        private readonly ISystemActivityService _activityService;
        private readonly IDepartmentService _departmentService;
        private readonly IDepartmentFactory _departmentfactory;
        private readonly IBranchService _branchService;
        private readonly IDepartmentUnitService _departmentUnitService;
        private readonly IProcessesService _processService;
        public OperationDashboardController(IApplicationLoggerFactory loggerFactory,
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
                                 IProcessesService processService,
                                 IOperationsDashboardFactory operationsDashboardFactory,
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
             _processService = processService;
            _operationsDashboardFactory = operationsDashboardFactory;
        }

        [LogActivityResult("User Login", "User logged in to the Operations Workflow Portal", ActivityTypeDefaults.USER_LOGIN, "SystemUser")]
        //[PermissionAuthorization(false, "VIEW_OPERATIONS", "OPERATIONS_DASHBOARD")]
        public async Task<IActionResult> Index() {
            OperationsDashboardModel model;
            UserModel currentUser = null;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATIONS-DASHBOARD-CONTROLLER", string.Empty);
                    if (User.Identity?.IsAuthenticated != true) {
                        model = await _operationsDashboardFactory.PrepareErrorOperationsDashboardModelAsync(currentUser);
                        return View(model);
                    }
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare operations dashboard
                model = await _operationsDashboardFactory.PrepareOperationsDashboardModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATIONS-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareErrorOperationsDashboardModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> TotalProcesses() {
            TotalExtensionModel model;
            UserModel currentUser = null;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-TOTALPROCESSES", string.Empty);
                    return RedirectToAction("Index");
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultTotalExtensionsModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-TOTALPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Completed() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-COMPLETEDPROCESSES" , string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.UpToDate.GetDescription());
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-COMPLETEDPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Proposed() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-PROPOSEDPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Proposed.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-PROPOSEDPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Dormant() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-DORMANTPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Dormant.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-DORMANTPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Cancelled() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CANCELLEDPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Cancelled.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CANCELLEDPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Unchanged() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-UNCHANGEDPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Unchanged.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-UNCHANGEDPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }
        
        public async Task<IActionResult> AddedProcesses() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-UNCHANGEDPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Draft.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-UNCHANGEDPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }
        
        public async Task<IActionResult> ReviewProcesses() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-UNCHANGEDPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Review.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-UNCHANGEDPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Due() {
            CategoryExtensionResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-DUEPROCESSES", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Due.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-DUEPROCESSES", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> AccountServices() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-ACCOUNTSERVICE", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.AccountServices.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-ACCOUNTSERVICE", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Reconciliation() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-RECONCILIATION", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.Reconciliation.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-RECONCILIATION", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> RecordsManagement() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-RECORDSMANAGEMENT", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.RecordsMgt.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-RECORDSMANAGEMENT", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Wallets() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-WALLETS", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.Wallets.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-WALLETS", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);

        }

        public async Task<IActionResult> Cash() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CASH", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.Cash.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CASH", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);

        }

        public async Task<IActionResult> Payments() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-PAYMENT", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.Payments.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-PAYMENT", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Channels()
        {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CHANNELS", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.Channels.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CHANNELS", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> CustomerExperience() {
            UnitExtensionCountResponse model;
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CUSTOMEREXP", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitExtensionsModelAsync(currentUser, OperationUnit.CustomerExp.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CUSTOMEREXP", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionUnitErrorModelAsync(currentUser);
            }

            return View(model);
        }

        
        public async Task<IActionResult> UnitCategoryProcesses([FromBody] UnitCategoryViewModel model) {
             try
             {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    Logger.LogActivity($"UNIT CATEGORY ERROR: Failed to retrieve current user record - {JsonSerializer.Serialize(grcResponse)}");
                }

                var currentUser = grcResponse.Data;
                if (currentUser == null)
                {
                    //..session has expired
                    return RedirectToAction("Login", "Application");
                }
                GrcUnitProcessRequest request = new() {
                    Unit =  GetUnitName(model.Unit),
                    Category = GetCategory(model.Category) ,
                    UserId = currentUser.UserId,
                    Action = "Retrieve unit-category processes",
                    IPAddress = ipAddress
                };

                //..get list of all processes
                var processList = await _processService.GetUnitProcessesAsync(request);

                List<GrcMiniProcessResponse> processes;
                if (processList.HasError)
                {
                    processes = new();
                    Logger.LogActivity($"PROCESSES DATA ERROR: Failed to retrieve processes - {JsonSerializer.Serialize(processList)}");
                }
                else
                {
                    processes = processList.Data;
                    Logger.LogActivity($"ROLE GROUP DATA - {JsonSerializer.Serialize(processes)}");
                }

                //..get ajax data
                List<object> listData = new();
                if (processes.Any()) {
                    listData = processes.Select(p => new {
                        id = p.Id,
                        processName = p.ProcessName,
                        description = p.Description,
                        Version = p.CurrentVersion,
                        status = p.Status
                    }).Cast<object>().ToList();
                }

                return Json(new { processes = listData });
            } catch (Exception ex) {
                Logger.LogActivity($"Error retrieving roles: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Json(new { processes = new List<object>() });
            }
        }

        #region Private methods

        private static string GetUnitName(string unit) {
            string name = unit switch {
                "Cash" => "Cash",
                "Channels" => "Channels",
                "Account Services" => "AccountServices",
                "Customer Experience" => "CustomerExp",
                "Reconciliation" => "Reconciliation",
                "Records Management" => "RecordsMgt",
                "Payment" => "Payments",
                _ => "Wallets",
            };
            return name;
        }

        private static string GetCategory(string category) {
            string name = category switch {
                "Draft" => "DRAFT",
                "UpToDate" => "UPTODATE",
                "Unchanged" => "UNCHANGED",
                "Proposed" => "PROPOSED",
                "Dormant" => "DORMANT",
                "Cancelled" => "CANCELLED",
                "On Hold" => "ONHOLD",
                "Obsolete" => "OBSOLETE",
                _ => "INREVIEW",
            };
            return name;
        }

        #endregion
    }
}
