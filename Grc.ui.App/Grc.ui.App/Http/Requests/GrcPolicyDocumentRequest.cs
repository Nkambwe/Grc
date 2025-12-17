using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {
    public class GrcPolicyDocumentRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("documentType")]
        public long DocumentTypeId { get; set; }

        [JsonPropertyName("documentStatus")]
        public string DocumentStatus { get; set; }

        [JsonPropertyName("responsibilityId")]
        public long ResponsibilityId { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("aligned")]
        public bool IsAligned { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("approvedBy")]
        public string ApprovedBy { get; set; }

        [JsonPropertyName("approvalDate")]
        public DateTime? ApprovalDate { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("lastReview")]
        public DateTime LastRevisionDate { get; set; }

        [JsonPropertyName("nextReview")]
        public DateTime? NextRevisionDate { get; set; }

        [JsonPropertyName("documentOwner")]
        public long OwnerId { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
