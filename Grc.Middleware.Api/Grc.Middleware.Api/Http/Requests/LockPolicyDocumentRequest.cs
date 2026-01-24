using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests
{
    public class LockPolicyDocumentRequest {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("isLocked")]
        public bool IsLocked { get; set; }

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }

}
