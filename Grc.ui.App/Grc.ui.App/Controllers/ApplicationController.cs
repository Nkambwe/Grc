using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Http;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IRegistrationFactory _registrationFactory;
        private readonly IInstallService _installService;
        private readonly ILoginFactory _loginFactory;

        public ApplicationController(IWebHelper webHelper,
                                     IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     IRegistrationFactory registrationFactory,
                                     ILocalizationService localizationService,
                                     IInstallService installService,
                                     IAuthenticationService authService,
                                     ISystemAccessService accessService,
                                     ILoginFactory loginFactory) :
            base(loggerFactory, environment, webHelper, localizationService) {
            _registrationFactory = registrationFactory;
            Logger.Channel = $"APPLICATION-{DateTime.Now:yyyyMMddHHmmss}";
            _installService = installService;
            _authService = authService;
            _accessService = accessService;
            _loginFactory = loginFactory;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpGet]
        public IActionResult Login() {
            var loginModel = _loginFactory.PrepareLoginModelAsync();
            return View(loginModel);
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        public virtual async Task<IActionResult> Login(LoginModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }

            var user = await _authService.AuthenticateAsync(model, WebHelper.GetCurrentIpAddress()); 
            if (user == null) {
                ModelState.AddModelError("", "Invalid login.");
                return View(model);
            }

            await _authService.SignInAsync(user, model.RememberMe);
            return RedirectToAction("Index", "Application");
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
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        public virtual async Task<IActionResult> Register(CompanyRegistrationModel model) {
            if (!ModelState.IsValid) {
                return HandleValidationErrors(model);
            }
    
            try {
                //..register company
                var grcResponse = await _installService.RegisterCompanyAsync(model,WebHelper.GetCurrentIpAddress());
                Logger.LogActivity($"REGISTER RESPONSE: {JsonSerializer.Serialize(grcResponse)}");
                if (grcResponse.HasError) {
                    return HandleServiceError(grcResponse, model);
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
            LocalizationService.SaveCurrentLanguage(language);
            return RedirectToAction("Register", "Application");
        }

        public  async Task<IActionResult> NoService(){
            var model = await _registrationFactory.PrepareNoServiceModelAsync();
            return View(model);
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

        private IActionResult HandleServiceError(GrcResponse<ServiceResponse> response, CompanyRegistrationModel model) {
            var error = response.Error;
            string errorMessage = $"{error.Code} - {error.Message}";
            Logger.LogActivity($"Registration failed: {errorMessage}");
    
            //..for Ajax call
             if (WebHelper.IsAjaxRequest(Request)) {
                Logger.LogActivity($"Sending to Ajax >>>>> ");
                return Json(response);
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
