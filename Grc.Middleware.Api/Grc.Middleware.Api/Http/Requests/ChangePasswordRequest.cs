using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {
    public class ChangePasswordRequest {

        [JsonPropertyName("id")]
        public long UserId { get; set; }
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }=string.Empty;
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
