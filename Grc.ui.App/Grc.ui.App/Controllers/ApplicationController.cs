using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {

        public ApplicationController(IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     ILogger<ApplicationController> aspNetLogger):
            base(loggerFactory, environment){
            Logger.Channel = $"APPLICATION-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public  IActionResult NoService(){ 
  
            try {
                if (!Environment.IsLive) {
                    Logger.LogActivity("Application - No service");
                } else {
                    Logger.LogActivity("Live Environment >>> Application - No service");
                }
            
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
            }
    
            return View();
        }

        public IActionResult Register() { 

            try {
                if (!Environment.IsLive) {
                    Logger.LogActivity("Application - Register");
                } else {
                    Logger.LogActivity("Live Environment >>> Application - Register");
                }
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
            }
    
            return View();
        }
    }
}
