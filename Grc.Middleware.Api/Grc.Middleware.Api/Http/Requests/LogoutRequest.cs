using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class LogoutRequest {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("isLoggedOut")]
        public bool IsLoggedOut { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        [JsonPropertyName("encrypts")]
        public string[] EncryptFields { get; set; }
        [JsonPropertyName("decrypts")]
        public string[] DecryptFields { get; set; }
    }
}
