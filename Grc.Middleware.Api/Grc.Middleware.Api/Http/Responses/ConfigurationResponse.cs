using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class ConfigurationResponse {

        [JsonPropertyName("general")]
        public GeneralConfigurations GeneralSettings { get; set; } = new();

        [JsonPropertyName("policies")]
        public PolicyConfigurations PolicySettings { get; set; } = new();

        [JsonPropertyName("audits")]
        public ComplianceAuditSettings AuditSettings { get; set; } = new();

        [JsonPropertyName("obligations")]
        public ObligationSettings ObligationSettings { get; set; } = new();

        [JsonPropertyName("mapping")]
        public MappingSettings MappingSettings { get; set; } = new();

        [JsonPropertyName("security")]
        public SecuritySettings SecuritySettings { get; set; } = new();
    }
}
