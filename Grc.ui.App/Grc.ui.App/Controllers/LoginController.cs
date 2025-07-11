using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {
    public class LoginController : GrcBaseController {

        public LoginController(IApplicationLoggerFactory loggerFactory, IEnvironmentProvider environment):
            base(loggerFactory, environment){
            Logger.Channel = $"{DateTime.Now:yyyyMMddHHmmss}-LOGIN";
        }


        public IActionResult UserLogin() {
            if (!Environment.IsLive) {
              Logger.LogActivity("Inside Login Controller Index");
            }
        
            return View();
        }

        public IActionResult Privacy() {
            if (!Environment.IsLive) {
              Logger.LogActivity("Calling Privacy endpoint");
            }
        
            return View();
        }

    }
}
