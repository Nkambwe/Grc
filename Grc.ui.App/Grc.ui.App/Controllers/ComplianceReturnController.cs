using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Grc.ui.App.Controllers {
    public class ComplianceReturnController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IDashboardFactory _dashboardFactory;

        public ComplianceReturnController(IApplicationLoggerFactory loggerFactory,
                                    IDashboardFactory dashboardFactory,
                                    IEnvironmentProvider environment,
                                    IWebHelper webHelper,
                                    ILocalizationService localizationService,
                                    IErrorService errorService,
                                    IAuthenticationService authService,
                                    IGrcErrorFactory errorFactory,
                                    SessionManager sessionManager)
                                    : base(loggerFactory, environment, webHelper,
                                          localizationService, errorService,
                                          errorFactory, sessionManager) {

            Logger.Channel = $"REGULATION-{DateTime.Now:yyyyMMddHHmmss}";
            _authService = authService;
            _dashboardFactory = dashboardFactory;
        }

        #region Regulatory Returns

        public async Task<IActionResult> ReturnsHome() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnsDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ReturnTotalStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnTotalStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ReturnOpenStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnOpenStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ReturnSubmittedStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnSubmittedStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ReturnBreachStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnBreachStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ReturnsRegister() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareReturnReceivedStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        #endregion

        #region Circular Returns

        public async Task<IActionResult> CircularHome() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> CircularReceivedStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularReceivedStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> CircularOpenStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularOpenStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> CircularClosedStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularClosedStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> CircularBreachStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareCircularBreachStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> CircularRegister() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel() {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        #endregion

        #region Return Tasks

        public async Task<IActionResult> ComplianceTasksHome() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareTasksDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> TotalTaskStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareTotalTaskStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> TaskOpenStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareOpenTaskStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> TaskClosedStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareClosedTaskStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> FailedTaskStatistics() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-RETURNS-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Login", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareFailedTaskStatisticModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> ComplianceTasksList() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel() {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        #endregion

    }
}
