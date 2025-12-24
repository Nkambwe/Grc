using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class ObligationMapViewModel {
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

        [JsonPropertyName("complianceMaps")]
        public List<GrcComplianceMapViewMode> ComplianceMaps { get; set; } = new();
    }
}
