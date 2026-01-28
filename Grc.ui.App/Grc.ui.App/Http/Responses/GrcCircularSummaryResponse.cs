using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    
    public class GrcCircularSummaryResponse {
        [JsonPropertyName("authorityAlias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("authorityName")]
        public string AuthorityName { get; set; }

        [JsonPropertyName("totalReceived")]
        public int TotalReceived { get; set; }

        [JsonPropertyName("closed")]
        public GrcCircularMetric Closed { get; set; }

        [JsonPropertyName("outstanding")]
        public GrcCircularMetric Outstanding { get; set; }

        [JsonPropertyName("breached")]
        public GrcCircularMetric Breached { get; set; }

        [JsonPropertyName("complianceRate")]
        public double ComplianceRate { get; set; } 
    }

}
