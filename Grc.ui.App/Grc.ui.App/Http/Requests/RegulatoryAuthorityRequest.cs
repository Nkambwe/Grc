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

        [JsonPropertyName("status")]
        public string Status { get; set; }
        public long UserId { get; set; }
        public string IPAddress { get; set; }
        public string Action { get; set; }
    }
}
