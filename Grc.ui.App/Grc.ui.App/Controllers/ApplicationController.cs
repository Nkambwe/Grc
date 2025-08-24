using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    public class ApplicationController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IRegistrationFactory _registrationFactory;
        private readonly IInstallService _installService;
        private readonly ILoginFactory _loginFactory;
        private readonly IDashboardFactory _dashboardFactory;

        public ApplicationController(IWebHelper webHelper,
                                     IApplicationLoggerFactory loggerFactory, 
                                     IEnvironmentProvider environment,
                                     IRegistrationFactory registrationFactory,
                                     ILocalizationService localizationService,
                                     IInstallService installService,
                                     IAuthenticationService authService,
                                     ISystemAccessService accessService,
                                     ILoginFactory loginFactory,
                                     IDashboardFactory dashboardFactory,
                                     IErrorService errorService,
                                     IGrcErrorFactory grcErrorFactory,
                                     SessionManager sessionManager) :
            base(loggerFactory, environment, webHelper, localizationService, 
                errorService, grcErrorFactory, sessionManager) {
            _registrationFactory = registrationFactory;
            Logger.Channel = $"APPLICATION-{DateTime.Now:yyyyMMddHHmmss}";
            _installService = installService;
            _authService = authService;
            _accessService = accessService;
            _loginFactory = loginFactory;
            _dashboardFactory = dashboardFactory;
        }

        public IActionResult Index() {
            if (User.Identity.IsAuthenticated) {
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                var redirectUrl = DetermineRedirectUrl(role);
                return Redirect(redirectUrl);
            } 

            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Dashboard() {

            try{
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    return RedirectToAction("Login");
                }

                return View(await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data));
            } catch(Exception ex){ 
                Logger.LogActivity($"Error Loading user dashboard: {ex.Message}", "ERROR");

                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-DASHBORAD", ex.StackTrace);
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login() {
            if (User.Identity?.IsAuthenticated == true) {
                return RedirectToAction("Index");
            }

            var loginModel = await _loginFactory.PrepareLoginModelAsync();
            return View(loginModel);
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        public virtual async Task<IActionResult> Login([FromBody] LoginModel model) {
            if (!ModelState.IsValid) {
                return HandleValidationErrors(model);
            }

            try {

                Logger.LogActivity($"Attempting authentication for user: {model.Username}");
                var response = await _authService.AuthenticateAsync(model, WebHelper.GetCurrentIpAddress());
                Logger.LogActivity($"REGISTER RESPONSE: {JsonSerializer.Serialize(response)}");
                if (response.HasError) {
                    return HandleLoginError(response, model);
                }

                await _authService.SignInAsync(response.Data, model.RememberMe);
                 Logger.LogActivity($"User successfully authenticated: {model.Username}");

                //..determine redirect URL based on user roles
                string redirectUrl = DetermineRedirectUrl(response.Data.RoleName);
                if (WebHelper.IsAjaxRequest(Request)) {
                    return Json(new {
                        success = true,
                        redirectUrl,
                        message = "Login successful"
                    });
                }

                 return Redirect(redirectUrl);
            } catch (Exception ex) {
                Logger.LogActivity($"{ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                var error = new GrcResponseError(
                    GrcStatusCodes.BADREQUEST,
                    LocalizationService.GetLocalizedLabel("Error.Occurance"),
                    "Could not complete login due to system error"
                );

                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-LOGIN", ex.StackTrace);
                Logger.LogActivity($"LOGIN ERROR: {JsonSerializer.Serialize(error)}");
                return HandleLoginError(new GrcResponse<UserModel>(error), model);
            }
        }
        
        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        public async Task<IActionResult> ValidateUsername(LoginModel model) {
            if (string.IsNullOrWhiteSpace(model.Username)) {
                return HandleUsernameValidationError(LocalizationService.GetLocalizedLabel("App.Message.EnterUsername"), model);
            }

            try {
                var grcResponse = await _accessService.ValidateUsernameAsync( await _loginFactory.PrepareUsernameValidationModelAsync(model.Username, WebHelper.GetCurrentIpAddress()));
                if (grcResponse.HasError) {
                     return HandleUsernameValidationError(grcResponse.Error.Message, model);
                }
                
                //..username is valid, prepare for password stage
                model.IsUsernameValidated = true;
                model.DisplayName = grcResponse.Data.DisplayName;
                model.CurrentStage = LoginStage.Password;

                if (WebHelper.IsAjaxRequest(Request)) {
                    return Json(new { 
                        success = true, 
                        displayName = grcResponse.Data.DisplayName,
                        message = LocalizationService.GetLocalizedLabel("App.Message.EnterPassword"),
                        stage = "password"
                    });
                }

                return View(model);
            } catch (GRCException ex) {
                Logger.LogActivity($"Username {model.Username} validation error: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");
                model.IsUsernameValidated = false;
                model.DisplayName = ex.Message;
                model.CurrentStage = LoginStage.Username;

                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-VALIDATEUSERNAME", ex.StackTrace);
                return HandleUsernameValidationError(LocalizationService.GetLocalizedLabel("Error.Service.Unavailable"), model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout() {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var username = User.Identity?.Name;
                Logger.LogActivity($"User logging out: {username}", "INFO");

                //..update logged_in status in database before signing out
                long id = 0;
                if (!string.IsNullOrEmpty(userId)) {
                    _ = long.TryParse(userId, out id);
                    await _accessService.UpdateLoggedInStatusAsync(id, false, ipAddress);
                }
                
                //..sign out from cookie authentication
                var model = await _loginFactory.PrepareLogoutModelAsync(id, ipAddress);
                await _authService.SignOutAsync(model);
            
                if (WebHelper.IsAjaxRequest(Request)) {
                    return Json(new { 
                        success = true, 
                        redirectUrl = Url.Action("Login", "Application")
                    });
                }
            
                return RedirectToAction("Login");
            } catch (Exception ex) {
                Logger.LogActivity($"Error during logout :: {ex.Message}" );
                Logger.LogActivity($"{ex.StackTrace}" );

                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-LOGOUT", ex.StackTrace);
                return RedirectToAction("Login");
            }
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
                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-REGISTER-COMPANY", ex.StackTrace);
                return HandleException(ex, model);
            }
        }

        [HttpGet]
        public virtual IActionResult ChangeLanguage(string language) {
            LocalizationService.SaveCurrentLanguage(language);
            return RedirectToAction("Register", "Application");
        }

        public  async Task<IActionResult> NoService(){
            var model = await _registrationFactory.PrepareNoServiceModelAsync();
            return View(model);
        }

        #region Helper Methods

        private IActionResult HandleValidationErrors(LoginModel model) {

            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors = errors });
            }
    
            //..for none Ajax call
            Notify(LocalizationService.GetLocalizedLabel("Error.Clear"), "GRC MESSAGE", NotificationType.Error);
            return View(model);
        }

        private IActionResult HandleValidationErrors(CompanyRegistrationModel model) {

            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
                return Json(new { success = false, errors = errors });
            }
    
            //..for none Ajax call
            Notify(LocalizationService.GetLocalizedLabel("Error.Clear"), "GRC MESSAGE", NotificationType.Error);
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
                : LocalizationService.GetLocalizedLabel("Error.Unknown");
        
            //..for Ajax call
            if (WebHelper.IsAjaxRequest(Request)) {
                var errors = new { general = new[] { errorMessage } };
                return Json(new { success = false, errors = errors });
            }
    
            //..for none Ajax call
            Notify(LocalizationService.GetLocalizedLabel("Registration.Error.Failed"), "GRC MESSAGE", NotificationType.Error);
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

        private IActionResult HandleUsernameValidationError(string errorMessage, LoginModel model) {
            if (WebHelper.IsAjaxRequest(Request)) {
                return Json(new { 
                    success = false, 
                    message = errorMessage,
                    stage = "username"
                });
            }
    
            ModelState.AddModelError("Username", errorMessage);
            return View(model);
        }

        private IActionResult HandleLoginError(GrcResponse<UserModel> response, LoginModel model) {
            var error = response.Error;
            string errorMessage = $"{error.Code} - {error.Message}";
            Logger.LogActivity($"Login failed: {errorMessage}");
            if (WebHelper.IsAjaxRequest(Request)) {
                Logger.LogActivity($"Sending to Ajax >>>>> ");
                return Json(response);
             }
    
            ModelState.AddModelError("Password", errorMessage);
            return View(model);
        }

        private string DetermineRedirectUrl(string roleName) {
            if(!string.IsNullOrWhiteSpace(roleName)){
                //..route to admin
                if (roleName.Equals("Administrator") || roleName.Equals("Support")) {
                    return Url.Action("Index", "Support", new { area = "Admin" });
                }

            }

            // Redirect to dashboard
            return Url.Action("Dashboard", "Application");
        }

        #endregion

    }
}
