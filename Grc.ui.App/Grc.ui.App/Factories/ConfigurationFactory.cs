using Grc.ui.App.Http.Responses;
using Grc.ui.App.Models;
using Grc.ui.App.Services;
using Grc.ui.App.Utils;
using Grc.ui.App.Extensions.Http;

namespace Grc.ui.App.Factories {
    public class ConfigurationFactory: IConfigurationFactory {

        private readonly SessionManager _sessionManager;
        private readonly ISystemConfiguration _configService;
        private readonly IAuditService _auditService;

        public ConfigurationFactory(ISystemConfiguration configService,
                                    IAuditService auditService,
                                    SessionManager sessionManager) { 
            _sessionManager = sessionManager;
            _configService = configService;
            _auditService = auditService;

        }
        public async Task<ConfigurationModel> PrepareConfigurationModelAsync(UserModel currentUser) {
            var grcResponse = await _configService.GetConfigurationAsync(currentUser.UserId, currentUser.IPAddress);
            GrcConfigurationResponse response = new();
            if (grcResponse.HasError) {
                response.GeneralSettings = new();
                response.PolicySettings = new();
                response.AuditSettings = new();
                response.ObligationSettings = new();
                response.MappingSettings = new();
                response.SecuritySettings = new();
            } else {
                var data = grcResponse.Data;
                response.GeneralSettings = data.GeneralSettings;
                response.PolicySettings = data.PolicySettings;
                response.AuditSettings = data.AuditSettings;
                response.ObligationSettings = data.ObligationSettings;
                response.MappingSettings = data.MappingSettings;
                response.SecuritySettings = data.SecuritySettings;
            }

            return new ConfigurationModel {
                WelcomeMessage = $"{currentUser?.FirstName} {currentUser?.LastName}",
                Initials = $"{currentUser?.LastName[..1]}{currentUser?.FirstName[..1]}",
                Workspace = _sessionManager.GetWorkspace(),
                GeneralSettings = response.GeneralSettings,
                PolicySettings = response.PolicySettings,
                AuditSettings = response.AuditSettings,
                ObligationSettings = response.ObligationSettings,
                MappingSettings = response.MappingSettings,
                SecuritySettings = response.SecuritySettings
            };
        }

    }
}
