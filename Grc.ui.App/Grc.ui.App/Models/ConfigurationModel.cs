using Grc.ui.App.Defaults;
using Grc.ui.App.Helpers;

namespace Grc.ui.App.Models {
    public class ConfigurationModel {
        public string Banner { get; } = CommonDefaults.AppVersion;
        public string WelcomeMessage { get; set; } = string.Empty;
        public string Initials { get; set; }
        public DateTime LastLogin { get; set; }
        public WorkspaceModel Workspace { get; set; }
        public GrcGeneralConfigurations GeneralSettings { get; set; }= new();
        public GrcPolicyConfigurations PolicySettings { get; set; }= new();
        public GrcComplianceAuditSettings AuditSettings { get; set; }= new();
        public GrcObligationSettings ObligationSettings { get; set; } = new();
        public GrcMappingSettings MappingSettings { get; set; } = new();
        public GrcSecuritySettings SecuritySettings { get; set; } = new();
    }
}
