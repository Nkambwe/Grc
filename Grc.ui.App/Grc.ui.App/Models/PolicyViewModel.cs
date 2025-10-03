using System.Text.Json.Serialization;

namespace Grc.ui.App.Models
{
    public class PolicyViewModel
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("doucmentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("typeId")]
        public long DocumentTypeId { get; set; }

        [JsonPropertyName("aligned")]
        public bool IsAligned { get; set; }

        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("underReview")]
        public bool UnderReview { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("ownerId")]
        public long OwnerId { get; set; }

        [JsonPropertyName("documentStatus")]
        public string DocumentStatus { get; set; }

        [JsonPropertyName("approverId")]
        public string Approver { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("lastReview")]
        public DateTime LastRevisionDate { get; set; }

        [JsonPropertyName("nextReview")]
        public DateTime? NextRevisionDate { get; set; }
        
        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
    }
}
