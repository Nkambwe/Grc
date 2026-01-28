using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class PeriodSummaryResponse {
        [JsonPropertyName("period")]
        public string Period { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("submitted")]
        public int Submitted { get; set; }

        [JsonPropertyName("submittedPercentage")]
        public double SubmittedPercentage { get; set; }

        [JsonPropertyName("pending")]
        public int Pending { get; set; }

        [JsonPropertyName("pendingPercentage")]
        public double PendingPercentage { get; set; }

        [JsonPropertyName("breached")]
        public int Breached { get; set; }

        [JsonPropertyName("breachedPercentage")]
        public double BreachedPercentage { get; set; }

        [JsonPropertyName("onTime")]
        public int OnTime { get; set; }

        [JsonPropertyName("onTimePercentage")]
        public double OnTimePercentage { get; set; }

        [JsonPropertyName("complianceRate")]
        public double ComplianceRate { get; set; }
    }

}
