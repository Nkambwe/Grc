using Grc.ui.App.Defaults;
using Grc.ui.App.Dtos;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare operations dashboard
                model = await _operationsDashboardFactory.PrepareOperationsDashboardModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATIONS-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareErrorOperationsDashboardModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> TotalProcesses() {
            var model = new TotalExtensionModel();
            UserModel currentUser = null;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CONTROLLER", string.Empty);
                    return RedirectToAction("Index");
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareDefaultTotalExtensionsModelAsync(currentUser);
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Completed() {
            var model = new CategoryExtensionModel();
            UserModel currentUser = null;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-DASHBOARD-CONTROLLER" , string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.UpToDate.GetDescription());
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-DASHBOARD-CONTROLLER" , ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Proposed() {
            var model = new CategoryExtensionModel();
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CONTROLLER", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Proposed.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Dormant() {
            var model = new CategoryExtensionModel();
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CONTROLLER", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Dormant.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Cancelled() {
            var model = new CategoryExtensionModel();
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CONTROLLER", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Cancelled.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Unchanged() {
            var model = new CategoryExtensionModel();
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CONTROLLER", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Unchanged.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> Due() {
            var model = new CategoryExtensionModel();
            UserModel currentUser = null;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CONTROLLER", string.Empty);
                    model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
                }

                currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareCategoryExtensionsModelAsync(currentUser, ProcessCategories.Due.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CONTROLLER", ex.StackTrace);
                model = await _operationsDashboardFactory.PrepareDefaultExtensionCategoryErrorModelAsync(currentUser);
            }

            return View(model);
        }

        public async Task<IActionResult> AccountServices() {
            OperationsDashboardModel model;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-ACCOUNTSERVICES", string.Empty);
                    return RedirectToAction("Index");
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.AccountServices.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-ACCOUNTSERVICES", ex.StackTrace);
                return RedirectToAction("Index");
                
            }

            return View(model);
        }

        public async Task<IActionResult> Reconciliation() {
            OperationsDashboardModel model;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-RECONCILIATION", string.Empty);
                    return RedirectToAction("Index");  
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.Reconciliation.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-RECONCILIATION", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> RecordsManagement() {
            OperationsDashboardModel model;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-RECORDSMANAGEMENT", string.Empty);
                    return RedirectToAction("Index");  
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.RecordsMgt.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-RECORDSMANAGEMENT", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Wallets() {
            OperationsDashboardModel model;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-WALLETS", string.Empty);
                    return RedirectToAction("Index");  
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.Wallets.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-WALLETS", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
            
        }

        public async Task<IActionResult> Cash() {
            OperationsDashboardModel model;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CASH", string.Empty);
                    return RedirectToAction("Index"); 
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.Cash.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CASH", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Payments() {
            OperationsDashboardModel model;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-PAYMENTS", string.Empty);
                    return RedirectToAction("Index");  
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.Payments.GetDescription());
            } catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-PAYMENTS", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> Channels()
        {
            OperationsDashboardModel model;
            try
            {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError)
                {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-PAYMENTS", string.Empty);
                    return RedirectToAction("Index");
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.Channels.GetDescription());
            }
            catch (Exception ex)
            {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-PAYMENTS", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<IActionResult> CustomerExperience() {
            OperationsDashboardModel model;
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message, "OPERATION-DASHBOARD-CUSTOMEREXPERIENCE", string.Empty);
                    return RedirectToAction("Index");
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _operationsDashboardFactory.PrepareUnitStatisticsModelAsync(currentUser, OperationUnit.CustomerExp.GetDescription());
            }
            catch (Exception ex) {
                await ProcessErrorAsync(ex.Message, "OPERATION-DASHBOARD-CUSTOMEREXPERIENCE", ex.StackTrace);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}
