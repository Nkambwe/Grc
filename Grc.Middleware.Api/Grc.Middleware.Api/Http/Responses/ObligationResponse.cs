using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class ObligationResponse {

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
        public List<ObligationComplianceMapResponse> ComplianceMaps { get; set; } = new();

        [JsonPropertyName("items")]
        public List<ComplianceIssueResponse> ComplianceIssues { get; set; } = new();

        [JsonPropertyName("revisions")]
        public List<ArticleRevisionResponse> Revisions { get; set; } = new();
    }
}
