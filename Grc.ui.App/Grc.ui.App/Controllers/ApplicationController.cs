using Grc.ui.App.Factories;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {

        private readonly IRegistrationFactory _registrationFactory;

        public ApplicationController(IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     IRegistrationFactory registrationFactory):
            base(loggerFactory, environment){
            _registrationFactory = registrationFactory;
            Logger.Channel = $"APPLICATION-{DateTime.Now:yyyyMMddHHmmss}";
        }
        
        public async Task<IActionResult> Register() { 
            var model = await _registrationFactory.PrepareCompanyRegistrationModelAsync();
            return View(model);
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

    }
}
