using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Responses {
    public class GrcRegulatoryAuthorityResponse {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("authorityName")]
        public string AuthorityName { get; set; }

        [JsonPropertyName("alias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("createdOn")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedOn")]
        public DateTime UpdatedAt { get; set; }
    }
}
