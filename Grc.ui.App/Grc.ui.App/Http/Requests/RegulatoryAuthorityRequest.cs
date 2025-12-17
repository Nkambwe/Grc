using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests
{
    public class RegulatoryAuthorityRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("authorityName")]
        public string AuthorityName { get; set; }

        [JsonPropertyName("authorityAlias")]
        public string AuthorityAlias { get; set; }

        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
