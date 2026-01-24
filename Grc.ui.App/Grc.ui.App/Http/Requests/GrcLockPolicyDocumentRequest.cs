using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class GrcLockPolicyDocumentRequest {

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
