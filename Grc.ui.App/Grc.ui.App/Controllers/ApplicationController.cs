using DocumentFormat.OpenXml.Office2010.Excel;
using Grc.ui.App.Defaults;
using Grc.ui.App.Enums;
using Grc.ui.App.Extensions.Http;
using Grc.ui.App.Factories;
using Grc.ui.App.Filters;
using Grc.ui.App.Helpers;
using Grc.ui.App.Http.Requests;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;
using System.Text.Json;

namespace Grc.ui.App.Controllers {

    [AreaAuthorization("COMPLIANCEDEPT", "COMPLIANCEADMIN", "COMPLIANCEGUESTS")]
    public class ApplicationController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IRegistrationFactory _registrationFactory;
        private readonly IInstallService _installService;
        private readonly ILoginFactory _loginFactory;
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IConfigurationFactory _configFactory;
        private readonly ISystemConfiguration _configService;
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
                                     IConfigurationFactory configFactory,
                                     IErrorService errorService,
                                     ISystemConfiguration configService,
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
            _configFactory = configFactory;
            _configService = configService;
        }

        [LogActivityResult("User Login", "User logged in to the system", ActivityTypeDefaults.USER_LOGIN, "SystemUser")]
        //[PermissionAuthorization(false, "VIEW_COMPLIANCE", "COMPLIANCE_DASHBOARD")]
        public async Task<IActionResult> Dashboard() {

            try{

                //..check if user is authenticated
                if(User.Identity?.IsAuthenticated != true) {
                    return RedirectToAction("Login");
                }

                //..get user role
                var roleGroup = User.FindFirst("RoleGroup")?.Value;
                if(roleGroup == null) {
                    //..we cannot determine user role, force login
                    return RedirectToAction("Login");
                }

                //..determine redirect URL based on user roles
                //..route to admin
                if (roleGroup.Equals("System Administrators") || roleGroup.Equals("Application Support"))
                {
                    return Redirect(Url.Action("Index", "Support", new { area = "Admin" }));
                }

                //..route to operations
                if (roleGroup.Equals("Operations")) {
                    return Redirect(Url.Action("Index", "OperationDashboard", new { area = "Operations" }));
                }

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    var model = new LoginModel() {
                        Username = "Unknown",
                        DisplayName = "Unknown",
                        CurrentStage = LoginStage.Username,
                    };
                    return HandleLoginError(new GrcResponse<UserModel>(grcResponse.Error), model);
                }

                var data = await _dashboardFactory.PrepareUserDashboardModelAsync(grcResponse.Data);
                data.WelcomeMessage = $"{data.WelcomeMessage} >> Dashboard";
                return View(data);
            } catch(Exception ex){ 
                Logger.LogActivity($"Error Loading user dashboard: {ex.Message}", "ERROR");
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-DASHBORAD", ex.StackTrace);
                var errModel = new GrcResponseError(500, "Error Loading user dashboard", "");
                var model = new LoginModel() {
                    Username = "Unknown",
                    DisplayName = "Unknown",
                    CurrentStage = LoginStage.Username,
                };
                Logger.LogActivity($"LOGIN ERROR: {JsonSerializer.Serialize(errModel)}");
                return HandleLoginError(new GrcResponse<UserModel>(errModel), model);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login() {
            var loginModel = await _loginFactory.PrepareLoginModelAsync();
            return View(loginModel);
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [AllowAnonymous]
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

                var data = response.Data;
                if(data == null) {
                    var error = new GrcResponseError(GrcStatusCodes.UNAUTHORIZED,
                        LocalizationService.GetLocalizedLabel("App.Authentication.Failed"),"Invalid login response from authentication service");
                    return HandleLoginError(new GrcResponse<UserModel>(error), model);
                }

                if (!data.IsAuthenticated)
                {
                    return HandleUsernameValidationError(LocalizationService.GetLocalizedLabel("App.Message.InvalidPassword"), model);
                }

                //..check password expiration
                bool passwordExpired = IsPasswordExpired(data);
                if (passwordExpired) {
                    return Json(new {
                        success = true,
                        stage = "password-expired",
                        message = "Password expired",
                        lastPasswordChange = data.LastPasswordChange
                    });
                }

                await _authService.SignInAsync(data, model.RememberMe);
                Logger.LogActivity($"User successfully authenticated: {model.Username}");

                //..determine redirect URL based on user roles
                string redirectUrl = DetermineRedirectUrl(response.Data.RoleGroup);
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
        [AllowAnonymous]
        public async Task<IActionResult> ValidateUsername(LoginModel model) {
            if (string.IsNullOrWhiteSpace(model.Username)) {
                return HandleUsernameValidationError(LocalizationService.GetLocalizedLabel("App.Message.EnterUsername"), model);
            }

            try {
                var grcResponse = await _accessService.ValidateUsernameAsync( await _loginFactory.PrepareUsernameValidationModelAsync(model.Username, WebHelper.GetCurrentIpAddress()));
                if (grcResponse.HasError) {
                     return HandleUsernameValidationError(grcResponse.Error.Message, model);
                }
                
                var data = grcResponse.Data;
                if(data == null || !data.IsValid) {
                    return HandleUsernameValidationError(LocalizationService.GetLocalizedLabel("App.Message.InvalidUsername"), model);
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
                return HandleUsernameValidationError(LocalizationService.GetLocalizedLabel("App.Authentication.Failed"), model);
            }
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [AllowAnonymous]
        public async Task<IActionResult> ChangeExpiredPassword([FromBody] ChangePasswordModel model){
            try {

                if(model == null){ 
                    return Json(new { success = false, message = "Invalid request object" });
                }
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Logger.LogActivity($"User: {model.Username} password change", "INFO");

                if (!long.TryParse(userId, out long id))
                    id = 0;

                var request = new GrcChangePasswordRequest(){ 
                    UserId = id,
                    RecordId = id,
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword,
                    Username = model.Username,
                    IPAddress = ipAddress,
                    Action = $"User password change for '{model.Username}'"
                    
                };

                var result = await _authService.ChangePasswordAsync(request);
                if (result.HasError)
                    return Json(new { success = false, message = result.Error.Message });

                return Json(new { success = true });
            } catch (GRCException ex) {
                Logger.LogActivity($"Password changevalidation error: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                //..capture error to bug tracker
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-PASSWORD-CHANGE", ex.StackTrace);
                return HandleUsernameValidationError("Password change failed. An error occurred", new LoginModel() {
                    Username = model.Username,
                    Password = string.Empty,
                    IsUsernameValidated = false,
                    DisplayName = string.Empty,
                });
            }
            
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword() {
             try{

                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    var model = new LoginModel() {
                        Username = "Unknown",
                        DisplayName = "Unknown",
                        CurrentStage = LoginStage.Username,
                    };
                    return HandleLoginError(new GrcResponse<UserModel>(grcResponse.Error), model);
                }

                var currentUser = grcResponse.Data;
                var data = await _dashboardFactory.PrepareUserPasswordModelAsync(currentUser);
                return View(data);
            } catch(Exception ex){ 
                Logger.LogActivity($"Error openning password change page: {ex.Message}", "ERROR");
                 _= await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-DASHBORAD", ex.StackTrace);
                var errModel = new GrcResponseError(500, "Error openning password change page", "");
                var model = new LoginModel() {
                    Username = "Unknown",
                    DisplayName = "Unknown",
                    CurrentStage = LoginStage.Username,
                };
                Logger.LogActivity($"LOGIN ERROR: {JsonSerializer.Serialize(errModel)}");
                return HandleLoginError(new GrcResponse<UserModel>(errModel), model);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PersistPassword([FromBody]ChangePasswordModel model) {
            try {

                if (model == null) {
                    return Json(new { success = false, message = "Invalid request object" });
                }
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Logger.LogActivity($"User: {model.Username} password change", "INFO");

                if (!long.TryParse(userId, out long id))
                    id = 0;

                var request = new GrcChangePasswordRequest() {
                    UserId = id,
                    RecordId = id,
                    OldPassword = model.OldPassword,
                    NewPassword = model.NewPassword,
                    Username = model.Username,
                    IPAddress = ipAddress,
                    Action = $"User password change for '{model.Username}'"

                };

                var result = await _authService.ChangePasswordAsync(request);
                if (result.HasError)
                    return Json(new { success = false, message = result.Error.Message });

                return Json(new { success = true, message="Password Successfully updated" });
            } catch (GRCException ex) {
                Logger.LogActivity($"Password reset error: {ex.Message}", "ERROR");
                Logger.LogActivity($"{ex.StackTrace}", "STACKTRACE");

                //..capture error to bug tracker
                _ = await ProcessErrorAsync(ex.Message, "APPLICATIONCONTROLLER-PASSWORD-CHANGE", ex.StackTrace);
                return HandleUsernameValidationError("Password change failed. An error occurred", new LoginModel() {
                    Username = model.Username,
                    Password = string.Empty,
                    IsUsernameValidated = false,
                    DisplayName = string.Empty,
                });
            }
        }

        [HttpPost]
        [LogActivityResult("User Logout", "User logged out of the system", ActivityTypeDefaults.USER_LOGOUT, "SystemUser")]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public virtual async Task<IActionResult> Register() { 
            var model = await _registrationFactory.PrepareCompanyRegistrationModelAsync();
            return View(model);
        }

        [HttpPost]
        [ServiceFilter(typeof(GrcAntiForgeryTokenAttribute))]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public virtual IActionResult ChangeLanguage(string language) {
            LocalizationService.SaveCurrentLanguage(language);
            return RedirectToAction("Register", "Application");
        }

        [AllowAnonymous]
        public  async Task<IActionResult> NoService(){
            var model = await _registrationFactory.PrepareNoServiceModelAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ComplianceSettings() {
            var ipAddress = WebHelper.GetCurrentIpAddress();
            var response = await _authService.GetCurrentUserAsync(ipAddress);
            if (response.HasError || response.Data == null) {
                return Redirect(Url.Action("Dashboard", "Application"));
            }

            return View(await _configFactory.PrepareConfigurationModelAsync(response.Data));
        }

        [HttpPost]
        public async Task<IActionResult> PasswordConfigurations([FromBody]PasswordConfigurationModel model) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (model == null) {
                    return BadRequest(new { success = false, message = "Invalid request data", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcPasswordConfigurationsRequest request = new() {
                    EnforcePasswordExpiration = model.ExpirePassword,
                    DaysUntilPasswordExpiration = model.ExipryDays,
                    MinimumPasswordLength = model.MinimumLength,
                    AllowManualPasswordReset = model.AllowMaualReset,
                    AllowPasswordReuse = model.AllowPwsReuse,
                    IncludeUppercaseCharacters = model.IncludeUpper,
                    IncludeLowercaseCharacters = model.IncludeLower,
                    IncludeSpecialCharacters = model.IncludeSpecial,
                    IncludeNumericCharacters = model.IncludeNumerics,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = "Update password policy Configurations"
                };

                var result = await _configService.SavePasswordPolicyConfigurationsAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to save settings" });


                var response = result.Data;
                if (!response.Status) {
                    return Ok(new { success = false, message = response.Message });
                }

                //..success
                return Ok(new { success = true, message = "Settings saved successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error save configurations: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "APPLICATION-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to save configurations.Something went wrong" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveGeneralConfigurations([FromBody] GeneralConfigurationModel model) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (model == null) {
                    return BadRequest(new { success = false, message = "Invalid request data", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcGeneralConfigurationsRequest request = new() {
                    SoftDeleteRecords = model.SoftDeleteRecords,
                    IncludeDeletedRecord = model.IncludeDeletedRecord,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = "Update General Configurations"
                };

                var result = await _configService.SaveGeneralConfigurationsAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to save settings" });


                var response = result.Data;
                if (!response.Status) {
                    return Ok(new { success = false, message = response.Message });
                }

                //..success
                return Ok(new { success = true, message = "Settings saved successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error save configurations: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "APPLICATION-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to save configurations.Something went wrong" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SavePolicyConfigurations([FromBody] PolicyConfigurationsModel model) {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var userResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (userResponse.HasError || userResponse.Data == null) {
                    var msg = "Unable to resolve current user";
                    Logger.LogActivity(msg);
                    return Ok(new { success = false, message = msg, data = new { } });
                }

                if (model == null) {
                    return BadRequest(new { success = false, message = "Invalid request data", data = new { } });
                }

                var currentUser = userResponse.Data;
                GrcPolicyConfigurationsRequest request = new() {
                    SendPolicyNotifications = model.SendPolicyNotifications,
                    MaximumNumberOfNotifications = model.MaximumNumberOfNotifications,
                    UserId = currentUser.UserId,
                    IPAddress = ipAddress,
                    Action = "Update Policy Configurations"
                };

                var result = await _configService.SavePolicyConfigurationsAsync(request);
                if (result.HasError || result.Data == null)
                    return Ok(new { success = false, message = result.Error?.Message ?? "Failed to save settings" });

                var response = result.Data;
                if(!response.Status) {
                    return Ok(new { success = false, message = response.Message });
                }

                //..success
                return Ok(new { success = true, message = "Settings saved successfully", data = new { } });

            } catch (Exception ex) {
                Logger.LogActivity($"Unexpected error save configurations: {ex.Message}", "ERROR");
                _ = await ProcessErrorAsync(ex.Message, "APPLICATION-CONTROLLER", ex.StackTrace);
                return Ok(new { success = false, message = "Unable to save configurations.Something went wrong" });
            }
        }

        #region Helper Methods
        private static bool IsPasswordExpired(UserModel data) {
            if (data.ForcePasswordChange)
                return true;

            if (!data.LastPasswordChange.HasValue)
                return true;

            return data.LastPasswordChange.Value.AddDays(30) < DateTime.UtcNow;
        }

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

        private string DetermineRedirectUrl(string roleGroup) {
            if (!string.IsNullOrWhiteSpace(roleGroup)) {
                //..route to admin
                if (RoleCategory.DEVELOPER.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.SYSTEM.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.ADMINSUPPORT.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.ADMINISTRATOR.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.APPLICATIONSUPPORT.ToString().Equals(roleGroup.ToUpper())) {
                    return Url.Action("Index", "Support", new { area = "Admin" });
                }

                //..route to operations
                if (RoleCategory.OPERATIONSERVICES.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.OPERATIONADMIN.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.OPERATIONGUESTS.ToString().Equals(roleGroup.ToUpper())) {
                    return Url.Action("Index", "OperationDashboard", new { area = "Operations" });
                }

                //..compliance
                if (RoleCategory.COMPLIANCEDEPT.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.COMPLIANCEADMIN.ToString().Equals(roleGroup.ToUpper()) ||
                    RoleCategory.COMPLIANCEGUESTS.ToString().Equals(roleGroup.ToUpper())) {
                    return Url.Action("Dashboard", "Application");
                }

            }

            //..redirect to login for unknow roles
            return Url.Action("Login", "Application");
        }

         public async Task<IActionResult> AccessDenied() {
            try {
                var ipAddress = WebHelper.GetCurrentIpAddress();
                var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                if (grcResponse.HasError) {
                    return Redirect("Login");
                }

                var currentUser = grcResponse.Data;
                var data = await _dashboardFactory.PrepareUserPasswordModelAsync(currentUser);
                return View(data);
            } catch (Exception ex){
                Logger.LogActivity($"Error retrieving role record: {ex.Message}", "ERROR");
                await ProcessErrorAsync(ex.Message, "SUPPORT-CONTROLLER", ex.StackTrace);
                return Redirect("Login");
            }
        }

        #endregion

    }
}
