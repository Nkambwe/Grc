using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularsResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("circularTitle")]
        public string CircularTitle { get; set; }

        [JsonPropertyName("requirement")]
        public string Requirement { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("recievedOn")]
        public DateTime RecievedOn { get; set; }

        [JsonPropertyName("deadlineOn")]
        public DateTime? DeadlineOn { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("isBreached")]
        public bool IsBreached { get; set; }

        [JsonPropertyName("breachReason")]
        public string BreachReason { get; set; }

        [JsonPropertyName("breachRisk")]
        public string BreachRisk { get; set; }

        [JsonPropertyName("refNumber")]
        public string RefNumber { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("issues")]
        public List<CircularIssueResponse> Issues { get; set; }

    }
}
