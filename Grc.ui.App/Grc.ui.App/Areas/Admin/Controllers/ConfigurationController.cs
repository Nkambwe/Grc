using Grc.ui.App.Http;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Admin.Controllers {

    [Area("Admin")]
    public class ConfigurationController: AdminBaseController {
        private readonly ISystemAccessService _accessService;
        private readonly IAuthenticationService _authService;

        public ConfigurationController(IApplicationLoggerFactory loggerFactory, 
                                 IEnvironmentProvider environment, 
                                 IWebHelper webHelper,
                                 ILocalizationService localizationService,
                                 ISystemAccessService accessService,
                                 IAuthenticationService authService) 
            : base(loggerFactory, environment, webHelper, localizationService) {
           _accessService = accessService;
            _authService = authService;
        }

        public async Task<IActionResult> Index() {
            return View();
        }


        public async Task<IActionResult> IPManagement() {
            return View();
        }

        public async Task<IActionResult> UserData() {
            return View();
        }
        
        public async Task<IActionResult> UserGroups() {
            return View();
        }
        
        public async Task<IActionResult> UserAuthentication() {
            return View();
        }
        
        public async Task<IActionResult> DataEncryptions() {
            return View();
        }
        
        public async Task<IActionResult> BugReporter() {
            return View();
        }
        
        public async Task<IActionResult> SystemActivity() {
            return View();
        }
    }
}
