using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {

        private readonly IRegistrationFactory _registrationFactory;
        private readonly ILocalizationService _localizationService;

        public ApplicationController(IWebHelper webHelper,
                                     IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     IRegistrationFactory registrationFactory,
                                     ILocalizationService localizationService):
            base(loggerFactory, environment, webHelper){
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

        public IActionResult Register(CompanyRegistrationModel model) {
            if (!ModelState.IsValid) {
                //..if it's an ajax request just return json with validation errors
                if (WebHelper.IsAjaxRequest(Request)) {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                    return Json(new { success = false, errors = errors });
                }
        
                //..for non-Ajax requests we return the view with validation errors
                Notify("Please clear the errors and Try again","GRC MESSAGE", NotificationType.Error);
                return View(model); 
            }
    
            try {
                // TODO: Save registration to database
                // Example:
                // await _registrationService.RegisterCompanyAsync(model);
        
                //..for ajax requests, return json success response
                if (WebHelper.IsAjaxRequest(Request)) {
                    return Json(new { 
                        success = true, 
                        redirectUrl = Url.Action("Login", "Application") 
                    });
                }
        
                //..for non-Ajax requests just redirect normally
                return RedirectToAction("Login", "Application");
            } catch (Exception ex) {
                //..log the exception
                Logger.LogActivity("Error during company registration");
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                string msg = "An error occurred during registration. Please try again.";
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") {
                    return Json(new { 
                        success = false, 
                        errors = new { general = new[] { msg } }
                    });
                }
                
                Notify(msg,"GRC MESSAGE", NotificationType.Error);
                ModelState.AddModelError("", msg);
                return View(model);
            }
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
