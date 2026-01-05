using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class CircularIssueRequest {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("issueDescription")]
        public string IssueDescription { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("receivedOn")]
        public DateTime RecievedOn { get; set; }

        [JsonPropertyName("resolvedOn")]
        public DateTime? ResolvedOn { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("circularId")]
        public long CircularId { get; set; }

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
