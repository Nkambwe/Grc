using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Operations.Controllers {

    [Area("Operations")]
    [Route("operations/workflow")]
    public class OperationWorkflowController : OperationsBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IOperationsDashboardFactory _dDashboardFactory;
        public OperationWorkflowController(IApplicationLoggerFactory loggerFactory, 
                                           IEnvironmentProvider environment,
                                           IWebHelper webHelper, 
                                           ILocalizationService localizationService,
                                           IErrorService errorService, 
                                           IOperationsDashboardFactory dDashboardFactory,
                                           IAuthenticationService authService,
                                           IGrcErrorFactory errorFactory,
                                           SessionManager sessionManager) 
                                        : base(loggerFactory, environment, webHelper, localizationService, 
                                              errorService, errorFactory, sessionManager) {
                                        _dDashboardFactory = dDashboardFactory;
                                        _authService = authService;
        }

        [HttpGet("registerProcess")]
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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("groupProcesses")]
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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("tagProcesses")]
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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("approvals")]
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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("pending")]
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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("revisions")]
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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-WORKFLOW-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

    }
}
