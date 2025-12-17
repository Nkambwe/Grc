using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class RegulatoryDocumentRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("documentName")]
        public string DocumentName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("approvedBy")]
        public string ApprovedBy { get; set; }

        [JsonPropertyName("policyAligned")]
        public bool PolicyAligned { get; set; }

        [JsonPropertyName("lastRevisionDate")]
        public DateTime LastRevisionDate { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("nextRevisionDate")]
        public DateTime? NextRevisionDate { get; set; }

        [JsonPropertyName("documentTypeId")]
        public long DocumentTypeId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("responsibilityId")]
        public long ResponsibilityId { get; set; }

        [JsonPropertyName("comments")]
        public string Comments { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("ceatedOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modifiedOn")]
        public DateTime ModifiedOn { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
