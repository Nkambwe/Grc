using System.Text.Json.Serialization;

namespace Grc.ui.App.Http.Requests {

    public class GrcChangePasswordRequest {

        [JsonPropertyName("userId")]
        public long UserId { get; set; }

        [JsonPropertyName("recordId")]
        public long RecordId { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }

        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }

        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }
    }

}
