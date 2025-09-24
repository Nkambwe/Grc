using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Admin.Controllers {

    [Area("Admin")]
    public class ConfigurationController: AdminBaseController {
        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;
        private readonly ISupportDashboardFactory _dDashboardFactory;

        public ConfigurationController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService,
                                 IErrorService errorService,
                                 IGrcErrorFactory errorFactory,
                                 ISupportDashboardFactory dDashboardFactory,
                                 SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, 
                  errorService, errorFactory, sessionManager) {
           _accessService = accessService;
            _authService = authService;
            _dDashboardFactory = dDashboardFactory;
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
                currentUser.LastLoginIpAddress = ipAddress;

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
                currentUser.LastLoginIpAddress = ipAddress;

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
                currentUser.LastLoginIpAddress = ipAddress;

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
                currentUser.LastLoginIpAddress = ipAddress;

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
                currentUser.LastLoginIpAddress = ipAddress;

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
                currentUser.LastLoginIpAddress = ipAddress;

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
                currentUser.LastLoginIpAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"SUPPORT-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
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
                currentUser.LastLoginIpAddress = ipAddress;

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
