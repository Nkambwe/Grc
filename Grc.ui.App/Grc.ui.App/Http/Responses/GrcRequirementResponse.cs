using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcRequirementResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("statute")]
        public string Statute { get; set; }

        [JsonPropertyName("section")]
        public string Section { get; set; }

        [JsonPropertyName("summery")]
        public string Summery { get; set; }

        [JsonPropertyName("obligation")]
        public string Obligation { get; set; }

        [JsonPropertyName("isMandatory")]
        public bool IsMandatory { get; set; }

        [JsonPropertyName("exclude")]
        public bool Exclude { get; set; }

        [JsonPropertyName("coverage")]
        public decimal Coverage { get; set; }

        [JsonPropertyName("isCovered")]
        public bool IsCovered { get; set; }

        [JsonPropertyName("assurance")]
        public decimal Assurance { get; set; }

        [JsonPropertyName("rationale")]
        public string ComplianceReason { get; set; }

        [JsonPropertyName("complianceMaps")]
        public List<GrcComplianceMapResponse> ComplianceMaps { get; set; } = new();
    }
}
