using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Responses
{
    public class RegulatoryAuthorityResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("authorityName")]
        public string AuthorityName { get; set; }

        [JsonPropertyName("alias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedOn { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedOn { get; set; }
    }
}
