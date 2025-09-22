using Grc.ui.App.Areas.Admin.Controllers;
using Grc.ui.App.Defaults;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Operations.Controllers {

    [Area("Operations")]
    [Route("operations/dashboard")]
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
            _operationsDashboardFactory = operationsDashboardFactory;
        }

        [LogActivityResult("User Login", "User logged in to the Operations Workflow Portal", ActivityTypeDefaults.USER_LOGIN, "SystemUser")]
        public async Task<IActionResult> Index() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATIONS-DASHBOARD-CONTROLLER", string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare operations dashboard
                model = await _operationsDashboardFactory.PrepareOperationsDashboardModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATIONS-DASHBOARD-CONTROLLER", ex.StackTrace);
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpPost("completed")]
        public async Task<IActionResult> Completed() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost("proposed")]
        public async Task<IActionResult> Proposed() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost("dormant")]
        public async Task<IActionResult> Dormant() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost("cancelled")]
        public async Task<IActionResult> Cancelled() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost("unchanged")]
        public async Task<IActionResult> Unchanged() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost("due")]
        public async Task<IActionResult> Due() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

    }
}
