using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Grc.ui.App.Controllers {

    public class ErrorController : GrcBaseController {

        public ErrorController(IWebHelper webHelper, 
                               IApplicationLoggerFactory loggerFactory, 
                               IEnvironmentProvider environment) 
            : base(loggerFactory, environment, webHelper) {
        }

        public IActionResult Status404() {
             if (!Environment.IsLive) {
              Logger.LogActivity("Inside Status 404 Endpoint");
            }
        
            return View();
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            Logger.LogActivity("Error! An error occurred");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
