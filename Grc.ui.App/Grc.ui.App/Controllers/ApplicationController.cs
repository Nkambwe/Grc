using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {

        private readonly IRegistrationFactory _registrationFactory;
        private readonly ILocalizationService _localizationService;
        private readonly IInstallService _installService;

        public ApplicationController(IWebHelper webHelper,
                                     IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     IRegistrationFactory registrationFactory,
                                     ILocalizationService localizationService,
                                     IInstallService installService) :
            base(loggerFactory, environment, webHelper) {
            _registrationFactory = registrationFactory;
            _localizationService = localizationService;
            Logger.Channel = $"APPLICATION-{DateTime.Now:yyyyMMddHHmmss}";
            _installService = installService;
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
        public IActionResult Login(LoginModel model) {
            //Notify("Failed to save data", "FAILED TO SAVE", NotificationType.Error);
            return View();
        }

        [HttpPost]
        public IActionResult Logout(LogoutModel model) {
            return View();
        }
        
        [HttpGet]
        public virtual async Task<IActionResult> Register() { 
            var model = await _registrationFactory.PrepareCompanyRegistrationModelAsync();
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Register(CompanyRegistrationModel model) {
            if (!ModelState.IsValid) {
                return HandleValidationErrors(model);
            }
    
            try {
                //..register company
                var grcResponse = await _installService.RegisterCompanyAsync(model);
                if (grcResponse.HasError) {
                    return HandleServiceError(grcResponse.Error, model);
                }
        
                //..success response
                var serviceResponse = grcResponse.Data;
                if (IsSuccessfulResponse(serviceResponse)) {
                    return HandleSuccess();
                }
        
                //..failed response
                return HandleServiceFailure(serviceResponse, model);
        
            } catch (Exception ex) {
                return HandleException(ex, model);
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

        #region Helper Methods

        private IActionResult HandleValidationErrors(CompanyRegistrationModel model) {

            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors = errors });
            }
    
            //..for none Ajax call
            Notify("Please clear the errors and Try again", "GRC MESSAGE", NotificationType.Error);
            return View(model);
        }

        private IActionResult HandleServiceError(GrcResponseError error, CompanyRegistrationModel model) {
            string errorMessage = $"{error.Code} - {error.Message}";
            Logger.LogActivity($"Registration failed: {errorMessage}");
    
            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                var errors = new { general = new[] { errorMessage } };
                return Json(new { success = false, errors = errors });
            }
    
            //..for none Ajax call
            Notify(error.Message, "GRC MESSAGE", NotificationType.Error);
            return View(model);
        }

        private IActionResult HandleSuccess() {
            if (WebHelper.IsAjaxRequest(Request)) {
                return Json(new { 
                    success = true, 
                    redirectUrl = Url.Action("Login", "Application") 
                });
            }
            return RedirectToAction("Login", "Application");
        }

        private IActionResult HandleServiceFailure(ServiceResponse serviceResponse, CompanyRegistrationModel model) {
            string errorMessage = serviceResponse != null 
                ? $"{serviceResponse.StatusCode} - {serviceResponse.Message}" 
                : "Unknown error occurred";
        
            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                var errors = new { general = new[] { errorMessage } };
                return Json(new { success = false, errors = errors });
            }
    
            //..for none Ajax call
            Notify("Registration failed. Please try again.", "GRC MESSAGE", NotificationType.Error);
            return View(model);
        }

        private IActionResult HandleException(Exception ex, CompanyRegistrationModel model) {
            Logger.LogActivity("Error during company registration");
            Logger.LogActivity($"{ex.Message}", "ERROR");
            Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
    
            string msg = "An error occurred during registration. Please try again.";
    
            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                return Json(new { 
                    success = false, 
                    errors = new { general = new[] { msg } }
                });
            }
    
            //..for none Ajax calls
            Notify(msg, "GRC MESSAGE", NotificationType.Error);
            ModelState.AddModelError("", msg);
            return View(model);
        }

        private bool IsSuccessfulResponse(ServiceResponse serviceResponse) {
            return serviceResponse != null && 
                    serviceResponse.Status && 
                    serviceResponse.StatusCode == (int)GrcStatusCodes.SUCCESS;
        }
        #endregion

    }
}
