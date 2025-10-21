using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Operations.Controllers {

    [Area("Operations")]
    [Route("operations/guests")]
    public class OperationsGuestController : OperationsBaseController {
        private readonly IAuthenticationService _authService;
        private readonly IOperationsDashboardFactory _dDashboardFactory;
        public OperationsGuestController(IApplicationLoggerFactory loggerFactory, 
                                        IEnvironmentProvider environment, 
                                        IWebHelper webHelper, 
                                        ILocalizationService localizationService, 
                                        IErrorService errorService, 
                                        IGrcErrorFactory errorFactory, 
                                        IOperationsDashboardFactory dDashboardFactory,
                                        IAuthenticationService authService,
                                        SessionManager sessionManager) 
                                        : base(loggerFactory, environment, webHelper, 
                                              localizationService, errorService, errorFactory, 
                                              sessionManager) {
                                        _dDashboardFactory = dDashboardFactory;
                                        _authService = authService;
        }

        [HttpPost("dashboard")]
        public async Task<IActionResult> Index() {
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
                currentUser.IPAddress = ipAddress;

                //..prepare user dashboard
                model = await _dDashboardFactory.PrepareDefaultOperationsModelAsync(currentUser);
            } catch(Exception ex){ 
                await ProcessErrorAsync(ex.Message,"OPERATION-SETTINGS-CONTROLLER" , ex.StackTrace);
                return View(model);
            }

            return View(model);
        }

        [HttpPost("approvalRequests")]
        public async Task<IActionResult> ApprovalRequests() {
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
                currentUser.IPAddress = ipAddress;

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
