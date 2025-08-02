using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {

    public class UserRequest {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("recordId")]
        public long RecordId { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
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
