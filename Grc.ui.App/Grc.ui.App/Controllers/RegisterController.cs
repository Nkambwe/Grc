using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {
    public class RegisterController : GrcBaseController
    {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;

        public RegisterController(IApplicationLoggerFactory loggerFactory,
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
            Logger.Channel = $"REGISTER-{DateTime.Now:yyyyMMddHHmmss}";
            _authService = authService;
        }

        public async Task<IActionResult> RegulationList()
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

        public async Task<IActionResult> RegulationObligations()
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
        
        public async Task<IActionResult> RegulationReturns() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }
        
        public async Task<IActionResult> ManageRegulations()
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

        public async Task<IActionResult> RegulationCirculars() {
            if (User.Identity?.IsAuthenticated == true) {
                var userDashboard = new UserDashboardModel()
                {
                    Initials = "JS",
                };

                return View(userDashboard);
            }

            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> RegulationMaps() {
            if (User.Identity?.IsAuthenticated == true) {
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
