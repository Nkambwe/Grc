using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class ObligationMapRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("assurance")]
        public decimal Assurance { get; set; }

        [JsonPropertyName("rationale")]
        public string Rationale { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("complianceMaps")]
        public List<ComplianceMapRequest> ComplianceMaps { get; set; } = new();
    }
}
