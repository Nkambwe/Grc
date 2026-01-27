using System.Text.Json.Serialization;

namespace Grc.Middleware.Api.Http.Requests {

    public class PasswordChangeRequest {
        /// <summary>
        /// Get or Set record ID
        /// </summary>
        [JsonPropertyName("recordId")]
        public long RecordId { get; set; }
        /// <summary>
        /// Get or Set user password
        /// </summary>
        [JsonPropertyName("oldPassword")]
        public string OldPassword { get; set; }
        /// <summary>
        /// Get or Set user password
        /// </summary>
        [JsonPropertyName("newPassword")]
        public string NewPassword { get; set; }
        /// <summary>
        /// Get or Set ID of user making request
        /// </summary>
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        /// <summary>
        /// Get or Set User IP Address
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IPAddress { get; set; }
        /// <summary>
        /// Get or Set Intended action
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }
    }
}
