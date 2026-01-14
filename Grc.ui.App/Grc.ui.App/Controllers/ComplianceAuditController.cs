using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class ComplianceAuditController : GrcBaseController {

        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IDashboardFactory _dashboardFactory;

        public ComplianceAuditController(IApplicationLoggerFactory loggerFactory, IEnvironmentProvider environment, IWebHelper webHelper, ILocalizationService localizationService, 
            IErrorService errorService, IAuthenticationService authService, IDashboardFactory dashboardFactory, IGrcErrorFactory errorFactory, SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, errorService, errorFactory, sessionManager) {
            Logger.Channel = $"AUDITS-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
            _dashboardFactory = dashboardFactory;
        }

        public async Task<IActionResult> AuditDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> MonthLessDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditExtensionDashboardModelAsync(grcResponse.Data, "MONTHLESS"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> TwoMonthsLessDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditExtensionDashboardModelAsync(grcResponse.Data, "TWOMONTHSLESS"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ThreeMonthsAboveDashboard() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("Dashboard", "Application"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareAuditExtensionDashboardModelAsync(grcResponse.Data, "THREEMONTHSABOVE"));
            }

            //..redirect to login
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> AuditExceptions() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("AuditHome", "ComplianceAudit"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }

        public async Task<IActionResult> AuditTypes() {
            if (User.Identity?.IsAuthenticated == true) {
                //..try getting current user logged in
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    //..log error to database
                    _ = await ProcessErrorAsync(grcResponse.Error.Message, "COMPLIACE-AUDIT-CONTROLLER", "Unable to process user information");
                    return Redirect(Url.Action("AuditHome", "ComplianceAudit"));
                }

                //..redirect to dashboard
                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            }

            //..redirect to login
            return Redirect(Url.Action("Login", "Application"));
        }
    }

}
