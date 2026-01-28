using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcCircularReportResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("authorityAlias")]
        public string AuthorityAlias { get; set; } = string.Empty;

        [JsonPropertyName("authority")]
        public string Authority { get; set; } = string.Empty;

        [JsonPropertyName("department")]
        public string Department { get; set; } = string.Empty;

        [JsonPropertyName("dueDate")]
        public DateTime? DueDate { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("isBreached")]
        public bool IsBreached { get; set; }

        [JsonPropertyName("breachRisk")]
        public string BreachRisk { get; set; } = string.Empty;

    }
}
