using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Grc.ui.App.Controllers {

    public class ErrorController : GrcBaseController {
        private readonly IErrorFactory _errorFactory;
        private readonly ILocalizationService _localizationService;
        public ErrorController(IWebHelper webHelper, 
                               IApplicationLoggerFactory loggerFactory, 
                               IEnvironmentProvider environment,
                               IErrorFactory errorFactory,
                               ILocalizationService localizationService) 
            : base(loggerFactory, environment, webHelper) {
            _errorFactory = errorFactory;
            _localizationService = localizationService;
        }

        public async Task<IActionResult> Status404() {
            Logger.LogActivity("Status 404 Endpoint", "NOTFOUND");
            var model = await _errorFactory.PrepareNotFoundModelAsync();
            return View(model);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            Logger.LogActivity(_localizationService.GetLocalizedLabel("Error.Occurance"));
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
