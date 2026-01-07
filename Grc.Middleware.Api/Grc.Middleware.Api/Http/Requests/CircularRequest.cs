using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class CircularRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("circularTitle")]
        public string CircularTitle { get; set; }

        [JsonPropertyName("requirement")]
        public string Requirement { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("receivedOn")]
        public DateTime RecievedOn { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("deadlineOn")]
        public DateTime DeadlineOn { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("closedOn")]
        public DateTime? ClosedOn { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
