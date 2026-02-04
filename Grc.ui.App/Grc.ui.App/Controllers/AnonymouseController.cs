using Grc.ui.App.Enums;
using Grc.ui.App.Factories;
using Grc.ui.App.Http.Responses;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Grc.ui.App.Controllers {
    public class AnonymouseController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;
        private readonly IRegistrationFactory _registrationFactory;
        private readonly IInstallService _installService;
        private readonly ILoginFactory _loginFactory;
        private readonly IDashboardFactory _dashboardFactory;
        private readonly IConfigurationFactory _configFactory;
        private readonly ISystemConfiguration _configService;
        public AnonymouseController(IWebHelper webHelper,
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

        public async Task<IActionResult> Index() {
            try{

                if (User.Identity?.IsAuthenticated == true) {
                    //..try getting current user logged in
                    var ipAddress = WebHelper.GetCurrentIpAddress();
                    var grcResponse = await _authService.GetCurrentUserAsync(ipAddress);
                    if (grcResponse.HasError) {
                        //..log error to database
                        _ = await ProcessErrorAsync(grcResponse.Error.Message, "ANOYNIMOUSE-CONTROLLER", "Unable to process user information");
                        return Redirect(Url.Action("Login", "Application"));
                    }

                    //..redirect to dashboard
                    return View(await _dashboardFactory.PrepareUserModelAsync(grcResponse.Data));
                }
                return Redirect(Url.Action("Login", "Application"));
            } catch(Exception ex){ 
                Logger.LogActivity($"Error loading user home page: {ex.Message}", "ERROR");
                 _= await ProcessErrorAsync(ex.Message, "ANOYNIMOUSE-DASHBORAD", ex.StackTrace);
                var errModel = new GrcResponseError(500, "Error loading user home page", "");
                var model = new LoginModel() { Username = "Anonymouse", DisplayName = "Unknown", CurrentStage = LoginStage.Username, };
                Logger.LogActivity($"LOGIN ERROR: {JsonSerializer.Serialize(errModel)}");
                return Redirect(Url.Action("Login", "Application"));
            }
        }
    }
}
