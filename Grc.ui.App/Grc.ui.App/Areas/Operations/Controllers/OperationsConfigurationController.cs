using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Operations.Controllers {

    [Area("Operations")]
    [Route("operations/settings")]
    public class OperationsConfigurationController : OperationsBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IOperationsDashboardFactory _dDashboardFactory;
        public OperationsConfigurationController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, 
            IWebHelper webHelper, 
            ILocalizationService localizationService, 
            IErrorService errorService, 
            IGrcErrorFactory errorFactory,
            IOperationsDashboardFactory dDashboardFactory,
            IAuthenticationService authService,
            SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, 
                  localizationService, errorService, 
                  errorFactory, sessionManager) {
            _dDashboardFactory = dDashboardFactory;
            _authService = authService;
        }

        [HttpGet("teams")]
        public async Task<IActionResult> Teams() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-SETTINGS-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("guests")]
        public async Task<IActionResult> Guests() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-SETTINGS-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("delegation")]
        public async Task<IActionResult> Delegation() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-SETTINGS-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("processes")]
        public async Task<IActionResult> Processes() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-SETTINGS-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("general")]
        public async Task<IActionResult> General() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-SETTINGS-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpGet("additional")]
        public async Task<IActionResult> Additional() {
            var model = new OperationsDashboardModel();
            try {
                //..get user IP address
                var ipAddress = WebHelper.GetCurrentIpAddress();

                //..get current authenticated user record
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    await ProcessErrorAsync(grcResponse.Error.Message,"OPERATION-SETTINGS-CONTROLLER" , string.Empty);
                    return View(model);
                }

                var currentUser = grcResponse.Data;
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

    }
}
