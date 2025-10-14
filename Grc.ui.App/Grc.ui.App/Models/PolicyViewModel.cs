using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class PolicyViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("documentType")]
        public long DocumentTypeId { get; set; }

        [JsonPropertyName("documentStatus")]
        public string DocumentStatus { get; set; }

        [JsonPropertyName("aligned")]
        public bool IsAligned { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("reviewPeriod")]
        public int ReviewPeriod { get; set; }

        [JsonPropertyName("reviewStatus")]
        public string ReviewStatus { get; set; }

        [JsonPropertyName("lastReview")]
        public DateTime LastRevisionDate { get; set; }

        [JsonPropertyName("nextReview")]
        public DateTime? NextRevisionDate { get; set; }

        [JsonPropertyName("documentOwner")]
        public long OwnerId { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
    }

}
