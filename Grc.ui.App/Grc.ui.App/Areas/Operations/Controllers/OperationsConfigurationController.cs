using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Operations.Controllers {

    public class OperationsConfigurationController : OperationsBaseController {

        public OperationsConfigurationController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, IWebHelper webHelper, 
            ILocalizationService localizationService, IErrorService errorService, 
            IGrcErrorFactory errorFactory, SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, errorService, errorFactory, sessionManager) {
        }

        public async Task<IActionResult> Teams() {
            return View();
        }

        public async Task<IActionResult> Guests() {
            return View();
        }

        public async Task<IActionResult> Delegation() {
            return View();
        }

        public async Task<IActionResult> Processes() {
            return View();
        }

        public async Task<IActionResult> General() {
            return View();
        }

        public async Task<IActionResult> Additional()
        {
            return View();
        }

    }
}
