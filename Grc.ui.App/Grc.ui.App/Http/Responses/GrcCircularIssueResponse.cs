using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {

    public class GrcCircularIssueResponse {

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
        public string OwnerId { get; set; }

        [JsonPropertyName("owner")]
        public string Owner { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }
    }

}
