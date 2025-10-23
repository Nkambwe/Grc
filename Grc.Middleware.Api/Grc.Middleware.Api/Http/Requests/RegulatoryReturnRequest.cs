using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class RegulatoryReturnRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("returnName")]
        public string ReturnName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("requirementDetails")]
        public string RequirementDetails { get; set; }

        [JsonPropertyName("frequencyInfo")]
        public string FrequencyInfo { get; set; }

        [JsonPropertyName("typeId")]
        public long TypeId { get; set; }

        [JsonPropertyName("frequencyId")]
        public long FrequencyId { get; set; }

        [JsonPropertyName("nextSubmissionDate")]
        public DateTime NextSubmissionDate { get; set; }

        [JsonPropertyName("submissionDate")]
        public DateTime LastSubmissionDate { get; set; }

        [JsonPropertyName("articleId")]
        public long ArticleId { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("responsibilityId")]
        public long ResponsibilityId { get; set; }

        [JsonPropertyName("complianceGap")]
        public string ComplianceGap { get; set; }

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
