using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses
{
    public class PolicyDocumentResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("documentStatus")]
        public string Status { get; set; }

        [JsonPropertyName("isAligned")]
        public bool IsAligned { get; set; }

        [JsonPropertyName("locked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("frequencyName")]
        public string FrequencyName { get; set; }

        [JsonPropertyName("documentTypeId")]
        public long DocumentTypeId { get; set; }

        [JsonPropertyName("documentTypeName")]
        public string DocumentTypeName { get; set; }

        [JsonPropertyName("responsibilityId")]
        public long ResponsibilityId { get; set; }

        [JsonPropertyName("responsibilityName")]
        public string ResponsibilityName { get; set; }

        [JsonPropertyName("departmentId")]
        public long DepartmentId { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("approvedBy")]
        public string ApprovedBy { get; set; }

        [JsonPropertyName("aprovalDate")]
        public DateTime? ApprovalDate { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("lastReview")]
        public DateTime LastRevisionDate { get; set; }

        [JsonPropertyName("nextReview")]
        public DateTime? NextRevisionDate { get; set; }
    }
}
