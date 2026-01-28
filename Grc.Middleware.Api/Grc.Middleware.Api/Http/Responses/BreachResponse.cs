using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class BreachResponse {

        [JsonPropertyName("reportName")]
        public string ReportName { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("dueDate")]
        public DateTime DueDate { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("associatedRisk")]
        public string AssociatedRisk { get; set; }

    }
}
