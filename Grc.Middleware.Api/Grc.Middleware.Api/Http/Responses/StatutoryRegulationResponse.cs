using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses {
    public class StatutoryRegulationResponse {

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("categoryId")]
        public long CategoryId { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("statutoryTypeId")]
        public long StatutoryTypeId { get; set; }

        [JsonPropertyName("statutoryType")]
        public string StatutoryType { get; set; }

        [JsonPropertyName("authorityId")]
        public long AuthorityId { get; set; }

        [JsonPropertyName("authorityName")]
        public string AuthorityName { get; set; }

        [JsonPropertyName("statutoryLawCode")]
        public string StatutoryLawCode { get; set; }

        [JsonPropertyName("statutoryLawName")]
        public string StatutoryLawName { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdBy")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("modifiedBy")]
        public string ModifiedBy { get; set; }
    }
}
