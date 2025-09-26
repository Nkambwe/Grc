using Grc.ui.App.Factories;
using Grc.ui.App.Infrastructure;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;

namespace Grc.ui.App.Controllers {
    public class ComplianceSettingsController : GrcBaseController {
        private readonly IAuthenticationService _authService;
        private readonly ISystemAccessService _accessService;

        public ComplianceSettingsController(IApplicationLoggerFactory loggerFactory, 
            IEnvironmentProvider environment, 
            IWebHelper webHelper, 
            ILocalizationService localizationService, 
            IErrorService errorService, 
            IAuthenticationService authService,
            IGrcErrorFactory errorFactory, 
            SessionManager sessionManager) 
            : base(loggerFactory, environment, webHelper, 
                  localizationService, errorService,
                  errorFactory, sessionManager) {

            Logger.Channel = $"COMP-SETTINGS-{DateTime.Now:yyyyMMddHHmmss}";
             _authService = authService;
        }

        //Index
        //Authorities
        //DocumentType
        //Users
        //Delegation
        //Departments
        //Responsibilities

    }
}
