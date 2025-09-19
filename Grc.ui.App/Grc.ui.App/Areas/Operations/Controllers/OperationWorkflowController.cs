using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Areas.Operations.Controllers {
    public class OperationWorkflowController : OperationsBaseController {

        public OperationWorkflowController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, IWebHelper webHelper, 
            ILocalizationService localizationService, IErrorService errorService, 
            IGrcErrorFactory errorFactory, SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, localizationService, errorService, errorFactory, sessionManager) {
        }

        public async Task<IActionResult> RegisterProcess()
        {
            return View();
        }
        public async Task<IActionResult> GroupProcesses()
        {
            return View();
        }

        public async Task<IActionResult> TagProcesses()
        {
            return View();
        }

        public async Task<IActionResult> Approvals()
        {
            return View();
        }

        public async Task<IActionResult> Pending()
        {
            return View();
        }

        public async Task<IActionResult> Revisions()
        {
            return View();
        }

    }
}
