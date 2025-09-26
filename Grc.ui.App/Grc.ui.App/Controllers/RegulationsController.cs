using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class RegulationsController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;

        public RegulationsController(IApplicationLoggerFactory loggerFactory,
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
        }

        public async Task<IActionResult> ReceivedRegulations() {
            if (User.Identity?.IsAuthenticated == true) {
                return View();
            }

            
            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> OpenRegulations()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
       
        public async Task<IActionResult> ClosedRegulations()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
        
        public async Task<IActionResult> RegulatoryApplicable()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
       
        public async Task<IActionResult> RegulatoryGaps()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
        
        public async Task<IActionResult> RegulatoryCovered()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
        
        public async Task<IActionResult> RegulatoryIssues()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
       
        public async Task<IActionResult> RegulatoryNotApplicable()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }
     
        public async Task<IActionResult> ManageRegulations()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> RegulationMaps()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }

        public async Task<IActionResult> RegulationCirculars()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return View();
            }


            return Redirect(Url.Action("Dashboard", "Application"));
        }

    }
}
