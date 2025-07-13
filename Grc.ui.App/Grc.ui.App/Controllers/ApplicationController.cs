
using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {

        private readonly IRegistrationFactory _registrationFactory;
        private readonly ILocalizationService _localizationService;

        public ApplicationController(IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     IRegistrationFactory registrationFactory,
                                     ILocalizationService localizationService):
            base(loggerFactory, environment){
            _registrationFactory = registrationFactory;
            _localizationService = localizationService;
            Logger.Channel = $"APPLICATION-{DateTime.Now:yyyyMMddHHmmss}";
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Login() {
            
            //Notify("Failed to save data", "FAILED TO SAVE", NotificationType.Error);
            return View();
        }

        [HttpPost]
        public IActionResult Logout() {
            return View();
        }
        
        [HttpGet]
        public virtual async Task<IActionResult> Register() { 
            var model = await _registrationFactory.PrepareCompanyRegistrationModelAsync();
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(CompanyRegistrationModel model) {
            if (!ModelState.IsValid) {
                 Notify("Invalid data. Please check your data and try again", "GRC VALIDATION", NotificationType.Error);
                return View(model); 
            }

            //..TODO --save reistration
            //..redirect to login
            return RedirectToAction("Login", "Application");
        }

        [HttpGet]
        public virtual IActionResult ChangeLanguage(string language) {
             //Notify("Successfully saved");
            _localizationService.SaveCurrentLanguage(language);
            return RedirectToAction("Register", "Application");
        }

        public  IActionResult NoService(){ 
            return View();
        }

    }
}
