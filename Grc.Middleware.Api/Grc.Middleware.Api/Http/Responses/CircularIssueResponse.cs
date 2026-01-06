using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class CircularIssueResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("circularId")]
        public long CircularId { get; set; }

        [JsonPropertyName("issueDescription")]
        public string IssueDescription { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("receivedOn")]
        public DateTime ReceivedOn { get; set; }

        [JsonPropertyName("resolvedOn")]
        public DateTime? ResolvedOn { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

    }
}
