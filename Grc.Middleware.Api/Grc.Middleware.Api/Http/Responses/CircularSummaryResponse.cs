using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularSummaryResponse {
        [JsonPropertyName("authorityAlias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("authorityName")]
        public string AuthorityName { get; set; }

        [JsonPropertyName("totalReceived")]
        public int TotalReceived { get; set; }

        [JsonPropertyName("closed")]
        public CircularMetric Closed { get; set; }

        [JsonPropertyName("outstanding")]
        public CircularMetric Outstanding { get; set; }

        [JsonPropertyName("breached")]
        public CircularMetric Breached { get; set; }

        [JsonPropertyName("complianceRate")]
        public double ComplianceRate { get; set; }
    }
}
