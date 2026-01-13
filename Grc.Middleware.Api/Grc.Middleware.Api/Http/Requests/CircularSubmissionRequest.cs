using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class CircularSubmissionRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("isBreached")]
        public bool IsBreached { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("filePath")]
        public string FilePath { get; set; }

        [JsonPropertyName("breachReason")]
        public string BreachReason { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }
}
