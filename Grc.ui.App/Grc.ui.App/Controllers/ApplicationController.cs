using Grc.ui.App.Factories;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

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
                return View(model); 
            }

            //..TODO --save reistration

            //..redirect to login
            return RedirectToAction("Login", "Application");
        }

        [HttpGet]
        public virtual IActionResult ChangeLanguage(string language) {
            _localizationService.SaveCurrentLanguage(language);
            return RedirectToAction("Register", "Application");
        }

        public  IActionResult NoService(){ 
            return View();
        }

    }
}
