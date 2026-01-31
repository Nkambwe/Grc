using Grc.ui.App.Helpers;
using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcConfigurationResponse {

        [JsonPropertyName("general")]
        public GrcGeneralConfigurations GeneralSettings { get; set; } = new();

        [JsonPropertyName("policies")]
        public GrcPolicyConfigurations PolicySettings { get; set; } = new();

        [JsonPropertyName("audits")]
        public GrcComplianceAuditSettings AuditSettings { get; set; } = new();

        [JsonPropertyName("obligations")]
        public GrcObligationSettings ObligationSettings { get; set; } = new();

        [JsonPropertyName("mapping")]
        public GrcMappingSettings MappingSettings { get; set; } = new();

        [JsonPropertyName("security")]
        public GrcSecuritySettings SecuritySettings { get; set; } = new();
    }

}
