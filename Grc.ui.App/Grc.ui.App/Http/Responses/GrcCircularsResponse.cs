using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class GrcCircularsResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("circularTitle")]
        public string CircularTitle { get; set; }

        [JsonPropertyName("requirement")]
        public string Requirement { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("authority")]
        public string Authority { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("frequency")]
        public string Frequency { get; set; }

        [JsonPropertyName("recievedOn")]
        public DateTime RecievedOn { get; set; }

        [JsonPropertyName("deadlineOn")]
        public DateTime? DeadlineOn { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("breachRisk")]
        public string BreachRisk { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("issues")]
        public virtual List<GrcCircularIssueResponse> Issues { get; set; } = new();

    }
}
