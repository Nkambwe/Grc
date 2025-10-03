using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class PolicyRegisterResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("documentType")]
        public string DocumentType { get; set; }

        [JsonPropertyName("aligned")]
        public bool IsAligned { get; set; }

        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("reviewStatus")]
        public string ReviewStatus { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("documentStatus")]
        public string DocumentStatus { get; set; }

        [JsonPropertyName("approverId")]
        public string Approver { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("lastReview")]
        public string LastRevisionDate { get; set; }

        [JsonPropertyName("nextReview")]
        public string NextRevisionDate { get; set; }
    }
}
