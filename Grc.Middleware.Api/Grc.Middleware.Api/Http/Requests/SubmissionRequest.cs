using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class SubmissionRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("file")]
        public string File { get; set; }
        
        [JsonPropertyName("breachReason")]
        public string BreachReason { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("submittedOn")]
        public DateTime? SubmittedOn { get; set; }

        [JsonPropertyName("submittedBy")]
        public string SubmittedBy { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
    }
}
