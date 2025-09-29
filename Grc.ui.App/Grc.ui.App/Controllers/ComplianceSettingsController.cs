using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {
    public class ComplianceSettingsController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;

        public ComplianceSettingsController(IApplicationLoggerFactory loggerFactory, 
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

            Logger.Channel = $"COMP-SETTINGS-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
        }

        public async Task<IActionResult> ComplianceRegulatoryTypes()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceRegulatoryCategories()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceAuthorities()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceDocumentType() {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceUsers() {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceDelegation()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceDepartments()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> ComplianceResponsibilities()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

    }
}
